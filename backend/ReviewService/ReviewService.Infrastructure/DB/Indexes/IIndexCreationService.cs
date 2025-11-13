using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewService.Infrastructure.DB.Indexes
{
    public interface IIndexCreationService
    {
        void CreateIndexes();
    }
}
