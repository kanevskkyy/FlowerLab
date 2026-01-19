using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.Specification;
using OrderService.Domain.Entities;
using OrderService.Domain.QueryParams;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace OrderService.DAL.Specification
{
    public class OrderSpecification : Specification<Order>
    {
        public OrderSpecification(OrderSpecificationParameters parameters)
        {
            Query.Include(o => o.Status)
                 .Include(o => o.Items)
                 .Include(o => o.OrderGifts)
                 .Include(o => o.DeliveryInformation);

            if (parameters.UserId.HasValue)
                Query.Where(o => o.UserId == parameters.UserId.Value);

            if (parameters.StatusId.HasValue)
                Query.Where(o => o.StatusId == parameters.StatusId.Value);

            if (parameters.GuestToken.HasValue)
                Query.Where(o => o.GuestToken == parameters.GuestToken.Value);

            if (parameters.BouquetId.HasValue)
            {
                Query.Where(o => o.Items.Any(i => i.BouquetId == parameters.BouquetId.Value));
            }


            if (!string.IsNullOrEmpty(parameters.Sort))
            {
                switch (parameters.Sort)
                {
                    case "DateAsc":
                        Query.OrderBy(o => o.CreatedAt);
                        break;
                    case "QtyDesc":
                        Query.OrderByDescending(o => o.Items.Sum(i => i.Count));
                        break;
                    case "QtyAsc":
                        Query.OrderBy(o => o.Items.Sum(i => i.Count));
                        break;
                    case "NameAsc":
                        Query.OrderBy(o => o.UserFirstName).ThenBy(o => o.UserLastName);
                        break;
                    case "NameDesc":
                        Query.OrderByDescending(o => o.UserFirstName).ThenByDescending(o => o.UserLastName);
                        break;
                    case "DateDesc":
                    default:
                        Query.OrderByDescending(o => o.CreatedAt);
                        break;
                }
            }
            else
            {
                Query.OrderByDescending(o => o.CreatedAt);
            }
        }
    }
}
