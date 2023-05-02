using Core.DataAccess.EntityFramework;
using DataAccess.Context.EntityFramework;
using Entities.Concrete;

namespace DataAccess.Repositories.CustomerRelationshipRepository
{
    public class EfCustomerRelationshipDal : EfEntityRepositoryBase<CustomerRelationship, SimpleContextDb>, ICustomerRelationshipDal
    {
    }
}
