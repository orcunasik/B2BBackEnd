using Entities.Concrete;
using FluentValidation;

namespace Business.Repositories.BasketRepository.Validation
{
    public class BasketValidator : AbstractValidator<Basket>
    {
        public BasketValidator()
        {
        }
    }
}
