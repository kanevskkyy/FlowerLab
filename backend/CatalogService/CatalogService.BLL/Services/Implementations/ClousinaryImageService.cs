using CatalogService.BLL.Services.Interfaces;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using CatalogService.BLL.DTO;

namespace CatalogService.BLL.Services.Implementations
{
    public class CloudinaryImageService : IImageService
    {
        private Cloudinary cloudinary;

        public CloudinaryImageService(IOptions<CloudSettings> options)
        {
            var settings = options.Value;
            Account account = new Account(settings.CloudName, settings.ApiKey, settings.ApiSecret);
            cloudinary = new Cloudinary(account);
            cloudinary.Api.Secure = true;
        }

        public async Task<string> UploadAsync(byte[] fileBytes, string fileName, string folder)
        {
            using var stream = new MemoryStream(fileBytes);
            return await UploadAsync(stream, fileName, folder);
        }

        public async Task<string> UploadAsync(Stream stream, string fileName, string folder)
        {
            if (stream.CanSeek) stream.Position = 0;

            ImageUploadParams uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(fileName, stream),
                Folder = folder,
                UseFilename = true,
                UniqueFilename = true,
                Overwrite = false
            };

            var result = await cloudinary.UploadAsync(uploadParams);

            if (result.StatusCode == System.Net.HttpStatusCode.OK)
                return result.SecureUrl.ToString();

            throw new Exception("Не вдалося завантажити зображення: " + result.Error?.Message);
        }

        public async Task<bool> DeleteAsync(string publicId)
        {
            var deletionParams = new DeletionParams(publicId);
            var result = await cloudinary.DestroyAsync(deletionParams);
            return result.Result == "ok" || result.Result == "not found";
        }
    }

}
