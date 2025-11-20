using CatalogService.Domain.Entities;
using CatalogService.Domain.QueryParametrs;
using System;
using System.Linq;

namespace CatalogService.DAL.Specification
{
    public class BouquetSpecification : BaseSpecification<Bouquet>
    {
        public BouquetSpecification(BouquetQueryParameters parameters)
            : base(b =>
                (!parameters.MinPrice.HasValue || b.Price >= parameters.MinPrice) &&
                (!parameters.MaxPrice.HasValue || b.Price <= parameters.MaxPrice) &&
                (!parameters.SizeIds.Any() || b.BouquetSizes.Any(s => parameters.SizeIds.Contains(s.SizeId))) &&
                (!parameters.EventIds.Any() || b.BouquetEvents.Any(e => parameters.EventIds.Contains(e.EventId))) &&
                (!parameters.RecipientIds.Any() || b.BouquetRecipients.Any(r => parameters.RecipientIds.Contains(r.RecipientId))) &&

                (!parameters.FlowerIds.Any() || parameters.FlowerIds.All(fId => b.BouquetFlowers.Any(bf => bf.FlowerId == fId)))

                && (!parameters.Quantities.Any() || parameters.Quantities.Contains(b.BouquetFlowers.Sum(bf => bf.Quantity)))

            )
        {
            AddInclude(b => b.BouquetFlowers);
            AddInclude(b => b.BouquetSizes);
            AddInclude(b => b.BouquetEvents);
            AddInclude(b => b.BouquetRecipients);
            AddInclude(b => b.BouquetImages);
        }
    }
}