using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Domain.Repositories;
using Ordering.Domain.Repositories.Base;
using Ordering.Infrastructure.Data;
using Ordering.Infrastructure.Repositories;
using Ordering.Infrastructure.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ordering.Infrastructure
{
   public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,IConfiguration configuration)
        {

            services.AddDbContext<OrderContext>(p => p.UseInMemoryDatabase(databaseName: "InMemoryDb"),
                                                ServiceLifetime.Singleton,
                                                ServiceLifetime.Singleton
                                                );

            services.AddTransient(typeof(IRepository<>),typeof(Repository<>));
            services.AddTransient<IOrderRepository, OrderRepository>();

            return services;
        }

    }
}
