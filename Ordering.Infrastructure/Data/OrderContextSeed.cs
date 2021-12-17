using Ordering.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Data
{
   public class OrderContextSeed
    {
        public static async Task SeedAsync(OrderContext orderContext)
        {
            if(!orderContext.Orders.Any()){
                orderContext.Orders.AddRange(GetPreconfiguredOrders());
                await orderContext.SaveChangesAsync();
            }
        }
        public static IEnumerable<Order> GetPreconfiguredOrders()
        {
            List<Order> orderList = new List<Order>();
            for (int i = 0; i < 5; i++)
            {
                Order order = new Order();
                order.AuctionId = Guid.NewGuid().ToString();
                order.ProductId = Guid.NewGuid().ToString();
                order.SellerUserName = $"{i}_test@test.com";
                order.UnitPrice =(i+1)*10;
                order.TotalPrice= (i + 1) * 100;
                order.CreateAt = DateTime.Now;

                orderList.Add(order);
            }
            return orderList;
        }
    }
}
