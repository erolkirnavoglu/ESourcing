using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Entities;
using Ordering.Domain.Repositories;
using Ordering.Infrastructure.Data;
using Ordering.Infrastructure.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Repositories
{
   public class OrderRepository:Repository<Order>,IOrderRepository
    {
        public OrderRepository(OrderContext context):base(context)
        {

        }

        public async Task<IEnumerable<Order>> GetOrdersBySellerUserName(string userName)
        {
          var orderList=  await _context.Orders
                  .Where(p => p.SellerUserName == userName)
                  .ToListAsync();
            return orderList;
        }
    }
}
