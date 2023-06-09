﻿using Business.Aspects.Secured;
using Business.Repositories.CustomerRepository.Constants;
using Business.Repositories.CustomerRepository.Validation;
using Business.Repositories.UserRepository;
using Core.Aspects.Caching;
using Core.Aspects.Performance;
using Core.Aspects.Validation;
using Core.Utilities.Business;
using Core.Utilities.Hashing;
using Core.Utilities.Result.Abstract;
using Core.Utilities.Result.Concrete;
using DataAccess.Repositories.CustomerRepository;
using Entities.Concrete;
using Entities.Dtos;

namespace Business.Repositories.CustomerRepository
{
    public class CustomerManager : ICustomerService
    {
        private readonly ICustomerDal _customerDal;

        public CustomerManager(ICustomerDal customerDal)
        {
            _customerDal = customerDal;
        }

        //[SecuredAspect()]
        [ValidationAspect(typeof(CustomerValidator))]
        [RemoveCacheAspect("ICustomerService.Get")]
        public async Task<IResult> Add(CustomerRegisterDto customerRegisterDto)
        {
            IResult result = BusinessRules.Run(
                await CheckIfEmailExists(customerRegisterDto.Email)
                );

            if (result is not null)
            {
                return result;
            }

            byte[] passwordHash, passwordSalt;
            HashingHelper.CreatePassword(customerRegisterDto.Password, out passwordHash, out passwordSalt);
            Customer customer = new Customer
            {
                Id = 0,
                Email = customerRegisterDto.Email,
                Name = customerRegisterDto.Name,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
            };
            await _customerDal.Add(customer);
            return new SuccessResult(CustomerMessages.Added);
        }

        [SecuredAspect()]
        [ValidationAspect(typeof(CustomerValidator))]
        [RemoveCacheAspect("ICustomerService.Get")]
        public async Task<IResult> Update(Customer customer)
        {
            await _customerDal.Update(customer);
            return new SuccessResult(CustomerMessages.Updated);
        }

        [SecuredAspect()]
        [RemoveCacheAspect("ICustomerService.Get")]
        public async Task<IResult> Delete(Customer customer)
        {
            await _customerDal.Delete(customer);
            return new SuccessResult(CustomerMessages.Deleted);
        }

        [SecuredAspect()]
        [CacheAspect()]
        [PerformanceAspect()]
        public async Task<IDataResult<List<Customer>>> GetList()
        {
            return new SuccessDataResult<List<Customer>>(await _customerDal.GetAll());
        }

        [SecuredAspect()]
        public async Task<IDataResult<Customer>> GetById(int id)
        {
            return new SuccessDataResult<Customer>(await _customerDal.Get(p => p.Id == id));
        }

        public async Task<Customer> GetByEmail(string email)
        {
            Customer result = await _customerDal.Get(p => p.Email == email);
            return result;
        }

        private async Task<IResult> CheckIfEmailExists(string email)
        {
            Customer customerList = await GetByEmail(email);
            if (customerList is not null)
            {
                return new ErrorResult("Bu mail adresi daha önce kullanılmış");
            }
            return new SuccessResult();
        }
    }
}