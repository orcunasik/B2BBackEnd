using Core.DataAccess;
using Entities.Concrete;

namespace DataAccess.Repositories.CustomerRepository
{
    public interface ICustomerDal : IEntityRepository<Customer>
    {
    }
}
