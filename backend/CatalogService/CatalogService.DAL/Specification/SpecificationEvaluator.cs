using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.DAL.Specification
{
    public static class SpecificationEvaluator<T> where T : class
    {
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, BaseSpecification<T> spec)
        {
            var query = inputQuery;

            if (spec.Criteria != null)
                query = query.Where(spec.Criteria);

            query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));

            if (spec.SortExpressions.Any())
            {
                IOrderedQueryable<T>? orderedQuery = null;

                for (int i = 0; i < spec.SortExpressions.Count; i++)
                {
                    var sort = spec.SortExpressions[i];
                    if (i == 0)
                    {
                        orderedQuery = sort.IsDescending 
                            ? query.OrderByDescending(sort.Expression) 
                            : query.OrderBy(sort.Expression);
                    }
                    else
                    {
                        orderedQuery = sort.IsDescending 
                            ? orderedQuery!.ThenByDescending(sort.Expression) 
                            : orderedQuery!.ThenBy(sort.Expression);
                    }
                }

                query = orderedQuery!;
            }

            return query;
        }
    }
}
