using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace MyShop.BLL.Services.AttachmentServices
{
    public interface IAttachmentServices
    {
        public Task<string?> UploadAsync(IFormFile file,string folderName);
        Task<bool> DeleteAsync(string filePath);
    }
}
