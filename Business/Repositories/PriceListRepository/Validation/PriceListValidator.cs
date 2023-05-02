using Entities.Concrete;
using FluentValidation;

namespace Business.Repositories.PriceListRepository.Validation
{
    public class PriceListValidator : AbstractValidator<PriceList>
    {
        public PriceListValidator()
        {
        }
    }
}
