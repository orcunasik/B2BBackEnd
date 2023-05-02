using Entities.Concrete;
using FluentValidation;

namespace Business.Repositories.ProductImageRepository.Validation
{
    public class ProductImageValidator : AbstractValidator<ProductImage>
    {
        public ProductImageValidator()
        {
        }
    }
}
