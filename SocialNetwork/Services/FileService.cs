using SocialNetwork.Assets.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SocialNetwork.Services
{
    public interface IFileService
    {
        Task<string> UploadAsync(IFormFile file, string pathToSave, string preFileToRemove = null, string defaultFileNameToPreventRemove = null);
        FileStream DownloadAsync(string fileFullName);
    }

    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<string> UploadAsync(IFormFile file, string pathToSave, string preFileToRemove = null, string defaultFileNameToPreventRemove = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_webHostEnvironment.WebRootPath))
                {
                    _webHostEnvironment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                }
                
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var directoryPath = Path.Combine(_webHostEnvironment.WebRootPath, pathToSave);
                var fileToCreateFullPath = Path.Combine(directoryPath, fileName);

                if(!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                using (var fileStream = new FileStream(fileToCreateFullPath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                if (!preFileToRemove.IsNullOrWhitespace() && preFileToRemove != defaultFileNameToPreventRemove)
                {
                    try
                    {
                        var fileToRemoveFullPath = Path.Combine(_webHostEnvironment.WebRootPath, pathToSave, preFileToRemove);
                        File.Delete(fileToRemoveFullPath);
                    }
                    catch { }
                }

                return fileName;
            }
            catch
            {
                return "";
            }
        }

        public FileStream DownloadAsync(string fileFullName)
        {
            fileFullName = Path.Combine(_webHostEnvironment.WebRootPath, fileFullName);
            if (File.Exists(fileFullName))
                return File.OpenRead(fileFullName);

            return null;
        }
    }
}
