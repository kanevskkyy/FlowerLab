using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.DAL.Specification
{
    public abstract class BaseSpecification<T> where T : class
    {
        public Expression<Func<T, bool>> Criteria { get; }
        public List<Expression<Func<T, object>>> Includes { get; } = new();

        public List<(Expression<Func<T, object>> Expression, bool IsDescending)> SortExpressions { get; } = new();

        protected BaseSpecification(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }

        protected void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }
        
        protected void AddOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            SortExpressions.Add((orderByExpression, false));
        }

        protected void AddOrderByDescending(Expression<Func<T, object>> orderByDescExpression)
        {
            SortExpressions.Add((orderByDescExpression, true));
        }
    }
}
