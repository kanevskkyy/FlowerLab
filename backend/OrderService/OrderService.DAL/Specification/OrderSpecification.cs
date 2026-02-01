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
                    .ThenInclude(i => i.Flowers)
                 .Include(o => o.OrderGifts)
                    .ThenInclude(og => og.Gift)
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

            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                var term = parameters.SearchTerm.ToLower();
                Query.Where(o =>
                    (o.UserFirstName != null && o.UserFirstName.ToLower().Contains(term)) ||
                    (o.UserLastName != null && o.UserLastName.ToLower().Contains(term)) ||
                    ((o.UserFirstName != null && o.UserLastName != null) && (o.UserFirstName.ToLower() + " " + o.UserLastName.ToLower()).Contains(term)) ||
                    (o.PhoneNumber != null && o.PhoneNumber.Contains(term)) ||
                    (o.ReceiverName != null && o.ReceiverName.ToLower().Contains(term)) ||
                    (o.ReceiverPhone != null && o.ReceiverPhone.Contains(term)) ||
                    o.Id.ToString().ToLower().Contains(term)
                );
            }

            if (!string.IsNullOrEmpty(parameters.Sort))
            {
                switch (parameters.Sort.ToLower())
                {
                    case "dateasc":
                        Query.OrderBy(o => o.CreatedAt).ThenBy(o => o.Id);
                        break;
                    case "datedesc":
                        Query.OrderByDescending(o => o.CreatedAt).ThenBy(o => o.Id);
                        break;
                    case "totalasc":
                        Query.OrderBy(o => o.TotalPrice).ThenBy(o => o.Id);
                        break;
                    case "totaldesc":
                        Query.OrderByDescending(o => o.TotalPrice).ThenBy(o => o.Id);
                        break;
                    case "statusasc":
                        Query.OrderBy(o => o.Status.Name == "AwaitingPayment" ? 0 :
                                          o.Status.Name == "Pending" ? 1 :
                                          o.Status.Name == "Processing" ? 2 :
                                          o.Status.Name == "Shipped" ? 3 :
                                          o.Status.Name == "Delivered" ? 4 :
                                          o.Status.Name == "Completed" ? 5 :
                                          o.Status.Name == "Refunded" ? 6 :
                                          o.Status.Name == "Cancelled" ? 7 : 10)
                             .ThenByDescending(o => o.CreatedAt)
                             .ThenBy(o => o.Id);
                        break;
                    case "qtydesc":
                        Query.OrderByDescending(o => o.Items.Sum(i => i.Count)).ThenBy(o => o.Id);
                        break;
                    case "qtyasc":
                        Query.OrderBy(o => o.Items.Sum(i => i.Count)).ThenBy(o => o.Id);
                        break;
                    case "nameasc":
                        Query.OrderBy(o => o.UserFirstName).ThenBy(o => o.UserLastName).ThenBy(o => o.Id);
                        break;
                    case "namedesc":
                        Query.OrderByDescending(o => o.UserFirstName).ThenByDescending(o => o.UserLastName).ThenBy(o => o.Id);
                        break;
                    default:
                        Query.OrderByDescending(o => o.CreatedAt).ThenBy(o => o.Id);
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
