using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Domain.QueryParams
{
    public class OrderSpecificationParameters
    {
        private const int MAX_PAGE_SIZE = 50;
        private int pageSize = 10;

        public int PageNumber { get; set; } = 1;

        public int PageSize
        {
            get => pageSize;
            set => pageSize = (value > MAX_PAGE_SIZE) ? MAX_PAGE_SIZE : value;
        }

        public Guid? UserId { get; set; }
        public Guid? StatusId { get; set; }
        public Guid? BouquetId { get; set; }

    }
}
