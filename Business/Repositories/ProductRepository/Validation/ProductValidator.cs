using Entities.Concrete;
using FluentValidation;

namespace Business.Repositories.ProductRepository.Validation
{
    public class ProductValidator : AbstractValidator<Product>
    {
        public ProductValidator()
        {
        }
    }
}
