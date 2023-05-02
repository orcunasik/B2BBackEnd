using Core.DataAccess.EntityFramework;
using DataAccess.Context.EntityFramework;
using Entities.Concrete;

namespace DataAccess.Repositories.ProductImageRepository
{
    public class EfProductImageDal : EfEntityRepositoryBase<ProductImage, SimpleContextDb>, IProductImageDal
    {
    }
}
