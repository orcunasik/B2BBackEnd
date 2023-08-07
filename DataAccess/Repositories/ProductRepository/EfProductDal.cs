using Core.DataAccess.EntityFramework;
using DataAccess.Context.EntityFramework;
using Entities.Concrete;
using Entities.Dtos;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.ProductRepository
{
    public class EfProductDal : EfEntityRepositoryBase<Product, SimpleContextDb>, IProductDal
    {
        public async Task<List<ProductListDto>> GetList()
        {
            using (SimpleContextDb context = new SimpleContextDb())
            {
                IQueryable<ProductListDto> result = from product in context.Products
                                                    select new ProductListDto
                                                    {
                                                        Id = product.Id,
                                                        Name = product.Name,
                                                        //Discount = 0,
                                                        //Price = 0,
                                                        MainImageUrl = (context.ProductImages.Where(p => p.ProductId == product.Id && p.IsMainImage == true).Count() > 0
                                                        ? context.ProductImages.Where(p => p.ProductId == product.Id && p.IsMainImage == true).Select(s => s.ImageUrl).FirstOrDefault()
                                                        : "")
                                                        //Images = context.ProductImages.Where(p => p.ProductId == product.Id).Select(s => s.ImageUrl).ToList()
                                                    };
                return await result.OrderBy(p => p.Name).ToListAsync();
            }
        }
        public async Task<List<ProductListDto>> GetProductList(int customerId)
        {
            using (SimpleContextDb context = new SimpleContextDb())
            {
                if (customerId != 0)
                {
                    var customerRelationship = context.CustomerRelationships.Where(c => c.CustomerId == customerId).SingleOrDefault();
                    var result = from product in context.Products
                                                        select new ProductListDto
                                                        {
                                                            Id = product.Id,
                                                            Name = product.Name,
                                                            Discount = customerRelationship.Discount,
                                                            Price = (context.PriceListDetails.Where(p => p.PriceListId == customerRelationship.PriceListId && p.ProductId == product.Id).Count() > 0
                                                            ? context.PriceListDetails.Where(p => p.PriceListId == customerRelationship.PriceListId && p.ProductId == product.Id).Select(s => s.Price).SingleOrDefault()
                                                            : 0),
                                                            MainImageUrl = (context.ProductImages.Where(p => p.ProductId == product.Id && p.IsMainImage == true).Count() > 0
                                                            ? context.ProductImages.Where(p => p.ProductId == product.Id && p.IsMainImage == true).Select(s => s.ImageUrl).FirstOrDefault()
                                                            : ""),
                                                            Images = context.ProductImages.Where(p => p.ProductId == product.Id).Select(s => s.ImageUrl).ToList()
                                                        };
                    return await result.OrderBy(p => p.Name).ToListAsync();
                }
                else
                {
                    var customerRelationship = context.CustomerRelationships.Where(c => c.CustomerId == customerId).SingleOrDefault();
                    var result = from product in context.Products
                                                        select new ProductListDto
                                                        {
                                                            Id = product.Id,
                                                            Name = product.Name,
                                                            Discount = 0,
                                                            Price = 0,
                                                            MainImageUrl = (context.ProductImages.Where(p => p.ProductId == product.Id && p.IsMainImage == true).Count() > 0
                                                            ? context.ProductImages.Where(p => p.ProductId == product.Id && p.IsMainImage == true).Select(s => s.ImageUrl).FirstOrDefault()
                                                            : ""),
                                                            Images = context.ProductImages.Where(p => p.ProductId == product.Id).Select(s => s.ImageUrl).ToList()
                                                        };
                    return await result.OrderBy(p => p.Name).ToListAsync();
                }
            }
        }
    }
}