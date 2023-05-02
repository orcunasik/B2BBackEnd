using Entities.Concrete;
using FluentValidation;

namespace Business.Repositories.PriceListDetailRepository.Validation
{
    public class PriceListDetailValidator : AbstractValidator<PriceListDetail>
    {
        public PriceListDetailValidator()
        {
        }
    }
}
