using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewService.Domain.Entities.QueryParameters
{
    public class ReviewQueryParameters : QueryStringParameters
    {
        public Guid? UserId { get; set; }
        public Guid? BouquetId { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int? Rating { get; set; }
        public ReviewStatus? Status { get; set; }
    }
}
