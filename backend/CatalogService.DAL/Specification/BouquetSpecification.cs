using CatalogService.Domain.Entities;
using CatalogService.Domain.QueryParametrs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.DAL.Specification
{
    public class BouquetSpecification : BaseSpecification<Bouquet>
    {
        public BouquetSpecification(BouquetQueryParameters parameters)
            : base(b =>
                (!parameters.MinPrice.HasValue || b.Price >= parameters.MinPrice) &&
                (!parameters.MaxPrice.HasValue || b.Price <= parameters.MaxPrice) &&
                (!parameters.MinFlowerCount.HasValue || b.BouquetFlowers.Sum(f => f.Quantity) >= parameters.MinFlowerCount) &&
                (!parameters.MaxFlowerCount.HasValue || b.BouquetFlowers.Sum(f => f.Quantity) <= parameters.MaxFlowerCount) &&
                (!parameters.SizeIds.Any() || b.BouquetSizes.Any(s => parameters.SizeIds.Contains(s.SizeId))) &&
                (!parameters.EventIds.Any() || b.BouquetEvents.Any(e => parameters.EventIds.Contains(e.EventId))) &&
                (!parameters.RecipientIds.Any() || b.BouquetRecipients.Any(r => parameters.RecipientIds.Contains(r.RecipientId))) &&
                (string.IsNullOrEmpty(parameters.FlowerName) || b.BouquetFlowers.Any(f => f.Flower.Name.Contains(parameters.FlowerName))) &&
                (string.IsNullOrEmpty(parameters.FlowerColor) || b.BouquetFlowers.Any(f => f.Flower.Color.Contains(parameters.FlowerColor)))
            )
        {
            AddInclude(b => b.BouquetFlowers);
            AddInclude(b => b.BouquetFlowers.Select(f => f.Flower));
            AddInclude(b => b.BouquetSizes);
            AddInclude(b => b.BouquetSizes.Select(s => s.Size));
            AddInclude(b => b.BouquetEvents);
            AddInclude(b => b.BouquetEvents.Select(e => e.Event));
            AddInclude(b => b.BouquetRecipients);
            AddInclude(b => b.BouquetRecipients.Select(r => r.Recipient));
            AddInclude(b => b.BouquetGifts);
            AddInclude(b => b.BouquetGifts.Select(g => g.Gift));
        }
    }
}
