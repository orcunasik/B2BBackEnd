using Business.Aspects.Secured;
using Business.Repositories.BasketRepository;
using Business.Repositories.OrderDetailRepository;
using Business.Repositories.PriceListDetailRepository;
using Business.Repositories.ProductImageRepository;
using Business.Repositories.ProductRepository.Constants;
using Business.Repositories.ProductRepository.Validation;
using Core.Aspects.Caching;
using Core.Aspects.Performance;
using Core.Aspects.Transaction;
using Core.Aspects.Validation;
using Core.Utilities.Business;
using Core.Utilities.Result.Abstract;
using Core.Utilities.Result.Concrete;
using DataAccess.Repositories.ProductRepository;
using Entities.Concrete;
using Entities.Dtos;

namespace Business.Repositories.ProductRepository
{
    public class ProductManager : IProductService
    {
        private readonly IProductDal _productDal;
        private readonly IProductImageService _productImageService;
        private readonly IPriceListDetailService _priceListDetailService;
        private readonly IBasketService _basketService;
        private readonly IOrderDetailService _orderDetailService;

        public ProductManager(IProductDal productDal, IProductImageService productImageService, IPriceListDetailService priceListDetailService, IBasketService basketService, IOrderDetailService orderDetailService)
        {
            _productDal = productDal;
            _productImageService = productImageService;
            _priceListDetailService = priceListDetailService;
            _basketService = basketService;
            _orderDetailService = orderDetailService;
        }

        [SecuredAspect("Admin,Product.Add")]
        [ValidationAspect(typeof(ProductValidator))]
        [RemoveCacheAspect("IProductService.Get")]
        public async Task<IResult> Add(Product product)
        {
            await _productDal.Add(product);
            return new SuccessResult(ProductMessages.Added);
        }

        [SecuredAspect("Admin,Product.Update")]
        [ValidationAspect(typeof(ProductValidator))]
        [RemoveCacheAspect("IProductService.Get")]
        public async Task<IResult> Update(Product product)
        {
            await _productDal.Update(product);
            return new SuccessResult(ProductMessages.Updated);
        }

        [SecuredAspect("Admin,Product.Delete")]
        [RemoveCacheAspect("IProductService.Get")]
        public async Task<IResult> Delete(Product product)
        {
            IResult result = BusinessRules.Run(
                await ChechkIfProductExistToBaskets(product.Id),
                await ChechkIfProductExistToOrderDetails(product.Id)
                );
            if (result is not null)
                return result;

            var productImages = await _productImageService.GetListByProductId(product.Id);
            foreach (ProductImage image in productImages.Data)
            {
                await _productImageService.Delete(image);
            }

            List<PriceListDetail> productPriceListDetails = await _priceListDetailService.GetListByProductId(product.Id);
            foreach (PriceListDetail priceListDetail in productPriceListDetails)
            {
                await _priceListDetailService.Delete(priceListDetail);
            }
            await _productDal.Delete(product);
            return new SuccessResult(ProductMessages.Deleted);
        }

        [SecuredAspect("Admin,Product.Get")]
        [CacheAspect()]
        [PerformanceAspect()]
        public async Task<IDataResult<List<ProductListDto>>> GetList()
        {
            return new SuccessDataResult<List<ProductListDto>>(await _productDal.GetList());
        }

        [SecuredAspect("Admin,Product.Get")]
        public async Task<IDataResult<Product>> GetById(int id)
        {
            return new SuccessDataResult<Product>(await _productDal.Get(p => p.Id == id));
        }

        [SecuredAspect("Admin,Product.Get")]
        [PerformanceAspect()]
        public async Task<IDataResult<List<ProductListDto>>> GetProductList(int customerId)
        {
            return new SuccessDataResult<List<ProductListDto>>(await _productDal.GetProductList(customerId));
        }

        public async Task<IResult> ChechkIfProductExistToBaskets(int productId)
        {
            List<Basket> result = await _basketService.GetListByProductId(productId);
            if (result.Count() > 0)
                return new ErrorResult("Silmeye Çalıştığınız Ürün Sepette Bulunuyor!");

            return new SuccessResult();
        }

        public async Task<IResult> ChechkIfProductExistToOrderDetails(int productId)
        {
            List<OrderDetail> result = await _orderDetailService.GetListByProductId(productId);
            if (result.Count() > 0)
                return new ErrorResult("Silmeye Çalıştığınız Ürün Sipariş Durumunda!");

            return new SuccessResult();
        }
    }
}