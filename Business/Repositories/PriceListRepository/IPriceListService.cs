using Core.Utilities.Result.Abstract;
using Entities.Concrete;
using Entities.Dtos;

namespace Business.Repositories.PriceListRepository
{
    public interface IPriceListService
    {
        Task<IResult> Add(PriceList priceList);
        Task<IResult> Update(PriceList priceList);
        Task<IResult> Delete(PriceList priceList);
        Task<IDataResult<List<PriceList>>> GetList();
        Task<IDataResult<PriceList>> GetById(int id);
    }
}
