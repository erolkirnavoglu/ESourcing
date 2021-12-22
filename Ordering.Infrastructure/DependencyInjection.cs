using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Infrastructure.Data;
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

            return services;
        }

    }
}
