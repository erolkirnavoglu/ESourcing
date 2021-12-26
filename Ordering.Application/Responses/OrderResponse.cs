using System;
using System.Collections.Generic;
using System.Text;

namespace Ordering.Application.Responses
{
   public class OrderResponse
    {
        public string AuctionId { get; set; }
        public string SellerUserName { get; set; }
        public string ProductId { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreateAt { get; set; }
    }
}
