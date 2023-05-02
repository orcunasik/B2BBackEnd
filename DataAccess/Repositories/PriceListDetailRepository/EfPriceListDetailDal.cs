using Core.DataAccess.EntityFramework;
using DataAccess.Context.EntityFramework;
using Entities.Concrete;

namespace DataAccess.Repositories.PriceListDetailRepository
{
    public class EfPriceListDetailDal : EfEntityRepositoryBase<PriceListDetail, SimpleContextDb>, IPriceListDetailDal
    {
    }
}
