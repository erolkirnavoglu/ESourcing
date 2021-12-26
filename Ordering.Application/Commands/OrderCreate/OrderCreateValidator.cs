using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ordering.Application.Commands.OrderCreate
{
    public class OrderCreateValidator : AbstractValidator<OrderCreateCommand>
    {
        public OrderCreateValidator()
        {
            RuleFor(p => p.SellerUserName)
                .EmailAddress()
                .NotEmpty();

            RuleFor(p => p.ProductId)
                .NotEmpty();
        }
    }
}
