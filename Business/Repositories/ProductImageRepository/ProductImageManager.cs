using Business.Abstract;
using Business.Aspects.Secured;
using Business.Repositories.ProductImageRepository.Constants;
using Business.Repositories.ProductImageRepository.Validation;
using Core.Aspects.Caching;
using Core.Aspects.Performance;
using Core.Aspects.Transaction;
using Core.Aspects.Validation;
using Core.Utilities.Business;
using Core.Utilities.Result.Abstract;
using Core.Utilities.Result.Concrete;
using DataAccess.Repositories.ProductImageRepository;
using Entities.Concrete;
using Entities.Dtos;
using System.IO;

namespace Business.Repositories.ProductImageRepository
{
    public class ProductImageManager : IProductImageService
    {
        private readonly IProductImageDal _productImageDal;
        private readonly IFileService _fileService;

        public ProductImageManager(IProductImageDal productImageDal, IFileService fileService)
        {
            _productImageDal = productImageDal;
            _fileService = fileService;
        }

        [SecuredAspect()]
        [ValidationAspect(typeof(ProductImageValidator))]
        [RemoveCacheAspect("IProductImageService.Get")]
        public async Task<IResult> Add(ProductImageAddDto productImageAddDto)
        {
            foreach (var image in productImageAddDto.Images)
            {
                IResult result = BusinessRules.Run(
                CheckIfImageExtesionsAllow(image.FileName),
                CheckIfImageSizeIsLessThanOneMb(image.Length)
                );

                if (result is null)
                {
                    string fileName = _fileService.FileSaveToServer(image, "C:/Users/nginit/source/repos/B2BFrontEnd/src/assets/admin/img/");
                    ProductImage productImage = new()
                    {
                        Id = 0,
                        ImageUrl = fileName,
                        ProductId = productImageAddDto.ProductId,
                        IsMainImage = false
                    };

                    await _productImageDal.Add(productImage);
                }
            }

            return new SuccessResult(ProductImageMessages.Added);
        }

        [SecuredAspect()]
        [ValidationAspect(typeof(ProductImageValidator))]
        [RemoveCacheAspect("IProductImageService.Get")]
        public async Task<IResult> Update(ProductImageUpdateDto productImageUpdateDto)
        {
            IResult result = BusinessRules.Run(
                CheckIfImageExtesionsAllow(productImageUpdateDto.Image.FileName),
                CheckIfImageSizeIsLessThanOneMb(productImageUpdateDto.Image.Length)
                );

            if (result is not null)
            {
                return result;
            }

            string imagePath = @"C:/Users/nginit/source/repos/B2BFrontEnd/src/assets/admin/img/" + productImageUpdateDto.ImageUrl;
            _fileService.FileDeleteToServer(imagePath);

            string fileName = _fileService.FileSaveToServer(productImageUpdateDto.Image, "./Content/img/");
            ProductImage productImage = new()
            {
                Id = productImageUpdateDto.Id,
                ImageUrl = fileName,
                ProductId = productImageUpdateDto.ProductId,
                IsMainImage = productImageUpdateDto.IsMainImage
            };

            await _productImageDal.Update(productImage);
            return new SuccessResult(ProductImageMessages.Updated);
        }

        [SecuredAspect()]
        [RemoveCacheAspect("IProductImageService.Get")]
        public async Task<IResult> Delete(ProductImage productImage)
        {
            string imagePath = @"C:/Users/nginit/source/repos/B2BFrontEnd/src/assets/admin/img/" + productImage.ImageUrl;
            _fileService.FileDeleteToServer(imagePath);

            await _productImageDal.Delete(productImage);
            return new SuccessResult(ProductImageMessages.Deleted);
        }

        [SecuredAspect()]
        [CacheAspect()]
        [PerformanceAspect()]
        public async Task<IDataResult<List<ProductImage>>> GetList()
        {
            return new SuccessDataResult<List<ProductImage>>(await _productImageDal.GetAll());
        }

        [SecuredAspect()]
        public async Task<IDataResult<ProductImage>> GetById(int id)
        {
            return new SuccessDataResult<ProductImage>(await _productImageDal.Get(p => p.Id == id));
        }

        private IResult CheckIfImageSizeIsLessThanOneMb(long imgSize)
        {
            decimal imgMbSize = Convert.ToDecimal(imgSize * 0.000001);
            if (imgMbSize > 5)
            {
                return new ErrorResult("Yüklediğiniz resmi boyutu en fazla 5mb olmalıdır");
            }
            return new SuccessResult();
        }

        private IResult CheckIfImageExtesionsAllow(string fileName)
        {
            string ext = fileName.Substring(fileName.LastIndexOf('.'));
            string extension = ext.ToLower();
            List<string> AllowFileExtensions = new List<string> { ".jpg", ".jpeg", ".gif", ".png" };
            if (!AllowFileExtensions.Contains(extension))
            {
                return new ErrorResult("Eklediğiniz resim .jpg, .jpeg, .gif, .png türlerinden biri olmalıdır!");
            }
            return new SuccessResult();
        }

        [SecuredAspect()]
        [RemoveCacheAspect("IProductImageService.Get")]
        [RemoveCacheAspect("IProductService.Get")]
        public async Task<IResult> SetMainImage(int id)
        {
            ProductImage productImage = await _productImageDal.Get(p => p.Id == id);
            List<ProductImage> productImages = await _productImageDal.GetAll(p => p.ProductId == productImage.ProductId);
            foreach (ProductImage imageItem in productImages)
            {
                imageItem.IsMainImage = false;
                await _productImageDal.Update(imageItem);
            };
            productImage.IsMainImage = true;
            await _productImageDal.Update(productImage);
            return new SuccessResult(ProductImageMessages.MainImageIsUpdated);
        }

        public async Task<IDataResult<List<ProductImage>>> GetListByProductId(int productId)
        {
            return new SuccessDataResult<List<ProductImage>>(await _productImageDal.GetAll(p => p.ProductId == productId));
        }
    }
}