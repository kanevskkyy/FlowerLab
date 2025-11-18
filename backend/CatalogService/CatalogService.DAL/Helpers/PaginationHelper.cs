using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.DAL.Helpers
{
    public static class PaginationHelper
    {
        public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> query, int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            return query.Skip((page - 1) * pageSize).Take(pageSize);
        }
    }
}
