using Entities.Concrete;
using FluentValidation;

namespace Business.Repositories.OrderRepository.Validation
{
    public class OrderValidator : AbstractValidator<Order>
    {
        public OrderValidator()
        {
        }
    }
}
