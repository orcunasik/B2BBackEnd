using Core.DataAccess.EntityFramework;
using DataAccess.Context.EntityFramework;
using Entities.Concrete;
using Entities.Dtos;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.OrderRepository
{
    public class EfOrderDal : EfEntityRepositoryBase<Order, SimpleContextDb>, IOrderDal
    {
        public async Task<List<OrderDto>> GetListDto()
        {
            using (SimpleContextDb context = new())
            {
                var result = from order in context.Orders
                             join customer in context.Customers on order.CustomerId equals customer.Id
                             select new OrderDto
                             {
                                 Id = order.Id,
                                 CustomerId = order.CustomerId,
                                 CustomerName = customer.Name,
                                 Date = order.Date,
                                 OrderNumber = order.OrderNumber,
                                 Status = order.Status,
                                 Quantity = context.OrderDetails.Where(p => p.OrderId == order.Id).Sum(s => s.Quantity),
                                 Total = context.OrderDetails.Where(p => p.OrderId == order.Id).Sum(s => s.Price) * context.OrderDetails.Where(p => p.OrderId == order.Id).Sum(s => s.Quantity)
                             };
                return await result.OrderByDescending(p => p.Id).ToListAsync();
            }
        }

        public async Task<OrderDto> GetByIdDto(int id)
        {
            using (SimpleContextDb context = new())
            {
                var result = from order in context.Orders.Where(p =>p.Id == id)
                             join customer in context.Customers on order.CustomerId equals customer.Id
                             select new OrderDto
                             {
                                 Id = order.Id,
                                 CustomerId = order.CustomerId,
                                 CustomerName = customer.Name,
                                 Date = order.Date,
                                 OrderNumber = order.OrderNumber,
                                 Status = order.Status,
                                 Quantity = context.OrderDetails.Where(p => p.OrderId == order.Id).Sum(s => s.Quantity),
                                 Total = context.OrderDetails.Where(p => p.OrderId == order.Id).Sum(s => s.Price) * context.OrderDetails.Where(p => p.OrderId == order.Id).Sum(s => s.Quantity)
                             };
                return await result.FirstOrDefaultAsync();
            }
        }

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
