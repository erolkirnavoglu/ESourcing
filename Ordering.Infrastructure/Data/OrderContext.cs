using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ordering.Infrastructure.Data
{
    public class OrderContext:DbContext
    {
        public OrderContext(DbContextOptions<OrderContext> dbContextOptions) : base(dbContextOptions)
        {

        }
        public DbSet<Order> Orders { get; set; }
    }
}
