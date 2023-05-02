using Entities.Concrete;
using FluentValidation;

namespace Business.Repositories.OrderDetailRepository.Validation
{
    public class OrderDetailValidator : AbstractValidator<OrderDetail>
    {
        public OrderDetailValidator()
        {
        }
    }
}
