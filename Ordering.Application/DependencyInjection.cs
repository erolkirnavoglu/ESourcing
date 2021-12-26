using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Application.Mapper;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Ordering.Application
{
   public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());

            var config = new MapperConfiguration(p => 
            {
                p.ShouldMapProperty = p => p.GetMethod.IsPublic || p.GetMethod.IsAssembly;
                p.AddProfile<OrderMappingProfile>();
            });
            var mapper = config.CreateMapper();


            return services;
        }
    }
}
