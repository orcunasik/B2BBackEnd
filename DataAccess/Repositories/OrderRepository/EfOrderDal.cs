using Core.DataAccess.EntityFramework;
using DataAccess.Context.EntityFramework;
using Entities.Concrete;

namespace DataAccess.Repositories.OrderRepository
{
    public class EfOrderDal : EfEntityRepositoryBase<Order, SimpleContextDb>, IOrderDal
    {
        public string GetOrderNumber()
        {
            using (SimpleContextDb context = new())
            {
                Order findLastOrder = context.Orders.OrderByDescending(o => o.Id).LastOrDefault();
                if (findLastOrder is null)
                    return "SP00000000000001";
                string findLastOrderNumber = findLastOrder.OrderNumber;
                 findLastOrderNumber = findLastOrderNumber.Substring(2, 14);
                int orderNumber = Convert.ToInt16(findLastOrderNumber);
                orderNumber++;
                string newOrderNumber = orderNumber.ToString();
                for (int i = newOrderNumber.Length; i < 14; i++)
                {
                    newOrderNumber = "0" + newOrderNumber;
                }
                newOrderNumber = "SP" + newOrderNumber;
                return newOrderNumber;
            }
        }
    }
}
