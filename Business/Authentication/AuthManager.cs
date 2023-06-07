using Business.Abstract;
using Business.Repositories.CustomerRepository;
using Business.Repositories.UserRepository;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Validation;
using Core.Utilities.Business;
using Core.Utilities.Hashing;
using Core.Utilities.Result.Abstract;
using Core.Utilities.Result.Concrete;
using Core.Utilities.Security.JWT;
using Entities.Concrete;
using Entities.Dtos;

namespace Business.Authentication
{
    public class AuthManager : IAuthService
    {
        private readonly IUserService _userService;
        private readonly ICustomerService _customerService;
        private readonly ITokenHandler _tokenHandler;

        public AuthManager(IUserService userService, ITokenHandler tokenHandler, ICustomerService customerService)
        {
            _userService = userService;
            _tokenHandler = tokenHandler;
            _customerService = customerService;
        }

        public async Task<IDataResult<AdminToken>> UserLogin(LoginAuthDto loginDto)
        {
            User user = await _userService.GetByEmail(loginDto.Email);
            if (user == null)
                return new ErrorDataResult<AdminToken>("Kullanıcı maili sistemde bulunamadı!");

            //if (!user.IsConfirm)
            //    return new ErrorDataResult<Token>("Kullanıcı maili onaylanmamış!");

            bool result = HashingHelper.VerifyPasswordHash(loginDto.Password, user.PasswordHash, user.PasswordSalt);
            List<OperationClaim> operationClaims = await _userService.GetUserOperationClaims(user.Id);

            if (result)
            {
                AdminToken token = new();
                token = _tokenHandler.CreateUserToken(user, operationClaims);
                return new SuccessDataResult<AdminToken>(token);
            }
            return new ErrorDataResult<AdminToken>("Kullanıcı maili ya da şifre bilgisi yanlış");
        }

        public async Task<IDataResult<CustomerToken>> CustomerLogin(CustomerLoginDto customerLoginDto)
        {
            Customer customer = await _customerService.GetByEmail(customerLoginDto.Email);
            if (customer is null)
                return new ErrorDataResult<CustomerToken>("Müşteri maili sistemde bulunamadı!");

            //if (!user.IsConfirm)
            //    return new ErrorDataResult<Token>("Kullanıcı maili onaylanmamış!");

            bool result = HashingHelper.VerifyPasswordHash(customerLoginDto.Password, customer.PasswordHash, customer.PasswordSalt);
            //List<OperationClaim> operationClaims = await _userService.GetUserOperationClaims(customer.Id);

            if (result)
            {
                CustomerToken token = new();
                token = _tokenHandler.CreateCustomerToken(customer);
                return new SuccessDataResult<CustomerToken>(token);
            }
            return new ErrorDataResult<CustomerToken>("Müşteri maili ya da şifre bilgisi yanlış");
        }

        [ValidationAspect(typeof(AuthValidator))]
        public async Task<IResult> Register(RegisterAuthDto registerDto)
        {
            IResult result = BusinessRules.Run(
                await CheckIfEmailExists(registerDto.Email),
                CheckIfImageExtesionsAllow(registerDto.Image.FileName),
                CheckIfImageSizeIsLessThanOneMb(registerDto.Image.Length)
                );

            if (result is not null)
            {
                return result;
            }

            await _userService.Add(registerDto);
            return new SuccessResult("Kullanıcı kaydı başarıyla tamamlandı");
        }

        private async Task<IResult> CheckIfEmailExists(string email)
        {
            User userList = await _userService.GetByEmail(email);
            if (userList is not null)
            {
                return new ErrorResult("Bu mail adresi daha önce kullanılmış");
            }
            return new SuccessResult();
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
    }
}