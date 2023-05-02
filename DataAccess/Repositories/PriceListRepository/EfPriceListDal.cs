using Core.DataAccess.EntityFramework;
using DataAccess.Context.EntityFramework;
using Entities.Concrete;

namespace DataAccess.Repositories.PriceListRepository
{
    public class EfPriceListDal : EfEntityRepositoryBase<PriceList, SimpleContextDb>, IPriceListDal
    {
    }
}
