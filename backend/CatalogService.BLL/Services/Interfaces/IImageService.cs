using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.BLL.Services.Interfaces
{
    public interface IImageService
    {
        
        Task<string> UploadAsync(byte[] fileBytes, string fileName, string folder);
        Task<bool> DeleteAsync(string publicId);
    }
}
