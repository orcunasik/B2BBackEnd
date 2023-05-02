using Entities.Concrete;
using FluentValidation;

namespace Business.Repositories.CustomerRelationshipRepository.Validation
{
    public class CustomerRelationshipValidator : AbstractValidator<CustomerRelationship>
    {
        public CustomerRelationshipValidator()
        {
        }
    }
}
