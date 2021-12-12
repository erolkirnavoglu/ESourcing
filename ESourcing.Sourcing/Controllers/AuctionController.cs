using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using ESourcing.Sourcing.Entities;
using ESourcing.Sourcing.Repositories.Interfaces;
using EventBusRabbitMQ.Core;
using EventBusRabbitMQ.Events;
using EventBusRabbitMQ.Producer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ESourcing.Sourcing.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    
    public class AuctionController : ControllerBase
    {
        private readonly IAuctionRepository _auctionRepository;
        private readonly ILogger<AuctionController> _logger;
        private readonly IBidRepository _bidRepository;
        private readonly IMapper _mapper;
        private readonly EventBusRabbitMQProducer _eventBus;
        public AuctionController(IAuctionRepository auctionRepository, IBidRepository bidRepository , ILogger<AuctionController> logger, IMapper mapper, EventBusRabbitMQProducer eventBus)
        {
            _auctionRepository = auctionRepository;
            _bidRepository = bidRepository;
            _logger = logger;
            _mapper = mapper;
            _eventBus = eventBus;
        }

        [HttpGet("[action]")]
        [ProducesResponseType(typeof(IEnumerable<Auction>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Auction>>> GetAuctions()
        {
            var auctions = await _auctionRepository.GetAuctions();
            return Ok(auctions);
        }

        [HttpGet("[action]")]
        [ProducesResponseType(typeof(Auction), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<Auction>> GetAuction(string id)
        {
            var auction = await _auctionRepository.GetAuction(id);
            if (auction == null)
            {
                _logger.LogError($" Auction with id: {id}, hasn't been found in database");
                return NotFound();
            }
            return Ok(auction);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Auction), (int)HttpStatusCode.Created)]
        public async Task<ActionResult<Auction>> CreateAuction([FromBody] Auction auction)
        {
            await _auctionRepository.Create(auction);
            return CreatedAtRoute("GetAuction", new { id = auction.Id }, auction);
        }

        [HttpPut]
        [ProducesResponseType(typeof(Auction), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Auction>> UpdateAuction([FromBody] Auction auction)
        {
            return Ok(await _auctionRepository.Update(auction));
        }

        [HttpDelete]
        [ProducesResponseType(typeof(Auction), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Auction>> DeleteAuction(string id)
        {
            return Ok(await _auctionRepository.Delete(id));
        }
        [HttpPost("[action]")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        public async Task<ActionResult> CompleteAuction(string id)
        {
            Auction auction = await _auctionRepository.GetAuction(id);
            if (auction == null) return NotFound();
            if (auction.Status != (int)Status.Active)
            {
                _logger.LogError("Auction can not be completed");
                return BadRequest();
            }
            Bid bid = await _bidRepository.GetWinnerBid(id);
            if (bid == null) return NotFound();

            OrderCreateEvent eventMessage = _mapper.Map<OrderCreateEvent>(bid);
            eventMessage.Quantity = auction.Quantity;
            auction.Status = (int)Status.Closed;

            bool updateResponse = await _auctionRepository.Update(auction);
            if (!updateResponse)
            {
                _logger.LogError("Auction can not response");
                return BadRequest();
            }

            try
            {
                _eventBus.Publish(EventBusConstants.OrderCreateQueue, eventMessage);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex,"Error publish integration event:{EventId} from {AppName}",eventMessage.Id,"Sourcing");
            }

            return Accepted();
        }
        [HttpPost("[action]")]
        public ActionResult<OrderCreateEvent> TestEvent()
        {
            OrderCreateEvent eventMessage = new OrderCreateEvent();
            eventMessage.AuctionId = "dummy1";
            eventMessage.ProductId = "dummy_product_1";
            eventMessage.Price = 10;
            eventMessage.Quantity = 100;
            eventMessage.SellerUserName = "test@gmail.com";

            try
            {
                _eventBus.Publish(EventBusConstants.OrderCreateQueue, eventMessage);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex,"Error publish integration event: {EventId} from {AppName}",eventMessage.Id,"Sourcing");
            }
            return Accepted(eventMessage);
        }

    }
}
