using CatalogService.Domain.Entities;
using CatalogService.Domain.QueryParametrs;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.DAL.Specification
{
    public class BouquetSpecification : BaseSpecification<Bouquet>
    {
        public BouquetSpecification(BouquetQueryParameters parameters)
            : base(b =>
                ((!parameters.MinPrice.HasValue && !parameters.MaxPrice.HasValue) ||
                 b.BouquetSizes.Any(bs =>
                     (!parameters.MinPrice.HasValue || bs.Price >= parameters.MinPrice) &&
                     (!parameters.MaxPrice.HasValue || bs.Price <= parameters.MaxPrice)
                 )) &&
                (!parameters.SizeIds.Any() || b.BouquetSizes.Any(s => parameters.SizeIds.Contains(s.SizeId))) &&
                (!parameters.EventIds.Any() || b.BouquetEvents.Any(e => parameters.EventIds.Contains(e.EventId))) &&
                (!parameters.RecipientIds.Any() || b.BouquetRecipients.Any(r => parameters.RecipientIds.Contains(r.RecipientId))) &&
                (!parameters.FlowerIds.Any() || 
                    b.BouquetFlowers.Any(bf => parameters.FlowerIds.Contains(bf.FlowerId)) || 
                    b.BouquetSizes.Any(bs => bs.BouquetSizeFlowers.Any(bsf => parameters.FlowerIds.Contains(bsf.FlowerId)))
                ) &&
                (!parameters.Quantities.Any() || parameters.Quantities.Contains(b.BouquetFlowers.Sum(bf => bf.Quantity))) &&
                (string.IsNullOrEmpty(parameters.Name) ||
                    EF.Functions.ILike(b.Name, $"%{parameters.Name}%") ||
                    b.BouquetFlowers.Any(bf => EF.Functions.ILike(bf.Flower.Name, $"%{parameters.Name}%"))
                )
            )
        {
            AddInclude(b => b.BouquetFlowers);
            AddInclude(b => b.BouquetSizes);
            AddInclude(b => b.BouquetEvents);
            AddInclude(b => b.BouquetRecipients);
            AddInclude(b => b.BouquetSizes);

            ApplySorting(parameters.SortBy);
        }

        private void ApplySorting(string sortBy)
        {
            switch (sortBy?.ToLower())
            {
                case "price_asc":
                    AddOrderBy(b => b.BouquetSizes.Min(bs => bs.Price));
                    break;

                case "price_desc":
                    AddOrderByDescending(b => b.BouquetSizes.Max(bs => bs.Price));
                    break;

                case "name_asc":
                    AddOrderBy(b => b.Name);
                    break;

                case "name_desc":
                    AddOrderByDescending(b => b.Name);
                    break;

                case "date_asc":
                    AddOrderBy(b => b.CreatedAt);
                    break;

                case "date_desc":
                case null:
                case "":
                    AddOrderByDescending(b => b.CreatedAt);
                    break;

                default:
                    AddOrderByDescending(b => b.CreatedAt);
                    break;
            }
        }
    }
}