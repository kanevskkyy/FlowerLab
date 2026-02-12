using CatalogService.Domain.Entities;
using CatalogService.Domain.QueryParametrs;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using CatalogService.DAL.Context;

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
                (!parameters.Quantities.Any() || 
                    parameters.Quantities.Contains(b.BouquetFlowers.Sum(bf => bf.Quantity)) ||
                    b.BouquetSizes.Any(bs => parameters.Quantities.Contains(bs.BouquetSizeFlowers.Sum(bsf => bsf.Quantity)))
                ) &&
                (string.IsNullOrEmpty(parameters.Name) ||
                    (CatalogDbContext.JsonExists(b.Name, "ua") && EF.Functions.ILike(CatalogDbContext.JsonExtractPathText(b.Name, "ua"), $"%{parameters.Name}%")) ||
                    (CatalogDbContext.JsonExists(b.Name, "en") && EF.Functions.ILike(CatalogDbContext.JsonExtractPathText(b.Name, "en"), $"%{parameters.Name}%")) ||
                    b.BouquetFlowers.Any(bf => 
                        (CatalogDbContext.JsonExists(bf.Flower.Name, "ua") && EF.Functions.ILike(CatalogDbContext.JsonExtractPathText(bf.Flower.Name, "ua"), $"%{parameters.Name}%")) || 
                        (CatalogDbContext.JsonExists(bf.Flower.Name, "en") && EF.Functions.ILike(CatalogDbContext.JsonExtractPathText(bf.Flower.Name, "en"), $"%{parameters.Name}%"))
                    ) ||
                    b.BouquetSizes.Any(bs => bs.BouquetSizeFlowers.Any(bsf => 
                        (CatalogDbContext.JsonExists(bsf.Flower.Name, "ua") && EF.Functions.ILike(CatalogDbContext.JsonExtractPathText(bsf.Flower.Name, "ua"), $"%{parameters.Name}%")) || 
                        (CatalogDbContext.JsonExists(bsf.Flower.Name, "en") && EF.Functions.ILike(CatalogDbContext.JsonExtractPathText(bsf.Flower.Name, "en"), $"%{parameters.Name}%"))
                    ))
                )
            )
        {
            AddInclude(b => b.BouquetFlowers);
            AddInclude(b => b.BouquetSizes);
            AddInclude(b => b.BouquetEvents);
            AddInclude(b => b.BouquetRecipients);

            ApplySorting(parameters);
        }

        private void ApplySorting(BouquetQueryParameters parameters)
        {
            if (!string.IsNullOrEmpty(parameters.Name))
            {
                // Prioritize Monobouquets (1 type of flower across global or per-size composition)
                AddOrderByDescending(b => 
                    (b.BouquetFlowers.Count == 1 && !b.BouquetSizes.Any(bs => bs.BouquetSizeFlowers.Any())) ||
                    (b.BouquetFlowers.Count == 0 && b.BouquetSizes.Any() && !b.BouquetSizes.Any(bs => bs.BouquetSizeFlowers.Count != 1))
                );

                // Sort by match quantity (sum of quantities of flowers that match the search term in both global and per-size composition)
                AddOrderByDescending(b => 
                    b.BouquetFlowers
                        .Where(bf => 
                            (CatalogDbContext.JsonExists(bf.Flower.Name, "ua") && EF.Functions.ILike(CatalogDbContext.JsonExtractPathText(bf.Flower.Name, "ua"), $"%{parameters.Name}%")) ||
                            (CatalogDbContext.JsonExists(bf.Flower.Name, "en") && EF.Functions.ILike(CatalogDbContext.JsonExtractPathText(bf.Flower.Name, "en"), $"%{parameters.Name}%"))
                        )
                        .Sum(bf => (int?)bf.Quantity) ?? 0 +
                    b.BouquetSizes
                        .SelectMany(bs => bs.BouquetSizeFlowers)
                        .Where(bsf => 
                            (CatalogDbContext.JsonExists(bsf.Flower.Name, "ua") && EF.Functions.ILike(CatalogDbContext.JsonExtractPathText(bsf.Flower.Name, "ua"), $"%{parameters.Name}%")) ||
                            (CatalogDbContext.JsonExists(bsf.Flower.Name, "en") && EF.Functions.ILike(CatalogDbContext.JsonExtractPathText(bsf.Flower.Name, "en"), $"%{parameters.Name}%"))
                        )
                        .Sum(bsf => (int?)bsf.Quantity) ?? 0);
            }

            switch (parameters.SortBy?.ToLower())
            {
                case "price_asc":
                    AddOrderBy(b => b.BouquetSizes.Min(bs => bs.Price));
                    break;

                case "price_desc":
                    AddOrderByDescending(b => b.BouquetSizes.Max(bs => bs.Price));
                    break;

                case "name_asc":
                    AddOrderBy(b => CatalogDbContext.JsonExtractPathText(b.Name, "ua")); // Default sort by UA
                    break;

                case "name_desc":
                    AddOrderByDescending(b => CatalogDbContext.JsonExtractPathText(b.Name, "ua"));
                    break;

                case "date_asc":
                    AddOrderBy(b => b.CreatedAt);
                    break;

                case "date_desc":
                case "popularity": // Map popularity to date_desc for now
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