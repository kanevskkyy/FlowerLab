using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewService.Domain.Entities.QueryParameters
{
    public class ReviewQueryParameters : QueryStringParameters
    {
        public Guid? UserId { get; set; }
        public Guid? BouquetId { get; set; }
        public int? Rating { get; set; }
        public ReviewStatus? Status { get; set; }
    }
}
