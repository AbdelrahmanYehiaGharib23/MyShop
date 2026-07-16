using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace MyShop.BLL.Services.AttachmentServices
{
    public class AttachmentService : IAttachmentServices
    {
        private static readonly List<string> allowedExtensions = new List<string> { ".png", ".jpg", ".jpeg" };
        private const int maxSize = 2_097_152; //2*1024*1024

        public async Task<string?> UploadAsync(IFormFile file, string folderName)
        {
            var extension = Path.GetExtension(file.FileName);
            if (!allowedExtensions.Contains(extension)) return null;
            if (file.Length == 0 || file.Length > maxSize) return null;
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folderName);
            Directory.CreateDirectory(folderPath);
            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(folderPath, fileName);
            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true))
            {
                await file.CopyToAsync(fs);
            }
            return Path.Combine(folderName, fileName).Replace("\\", "/"); ;
        }
        public async Task<bool> DeleteAsync(string relativePath)
        {
            try
            {
                var fullPath = Path.Combine(
             Directory.GetCurrentDirectory(),
             "wwwroot",
             relativePath.Replace("/", Path.DirectorySeparatorChar.ToString()));

                if (!File.Exists(fullPath))
                    return false;

                await Task.Run(() => File.Delete(fullPath));
                return true;
            }
            catch
            {
                return false;
            }
        }

     
    }
}
