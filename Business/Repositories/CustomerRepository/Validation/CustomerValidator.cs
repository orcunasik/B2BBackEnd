using Entities.Concrete;
using FluentValidation;

namespace Business.Repositories.CustomerRepository.Validation
{
    public class CustomerValidator : AbstractValidator<Customer>
    {
        public CustomerValidator()
        {
        }
    }
}
