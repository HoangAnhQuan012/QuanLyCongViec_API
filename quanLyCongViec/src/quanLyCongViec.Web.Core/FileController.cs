using Abp.Auditing;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using quanLyCongViec.Controllers;
using quanLyCongViec.Data.Excel.Dtos;
using quanLyCongViec.Roles.Dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quanLyCongViec
{
    public class FileController : quanLyCongViecControllerBase
    {
        private IAppFolder _appFolder;
        public FileController(IAppFolder appFolder)
        {
            _appFolder = appFolder;
        }

        [DisableAuditing]
        public ActionResult DownloadTempFile(FileDto file)
        {
            List<string> allowedExtensions = new List<string>
            {
                ".doc", ".docx", ".xls", ".xlsx", ".pdf", ".png", ".jpg"
            };

            string fileExtension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
            {
                throw new Exception("Invalid name!");
            }

            var filePath = Path.Combine(_appFolder.TempFileDownloadFolder, file.FileToken);
            if (!System.IO.File.Exists(filePath))
            {
                throw new UserFriendlyException(L("RequestedFileDoesNotExists"));
            }


            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            System.IO.File.Delete(filePath);
            return File(fileBytes, file.FileType, file.FileName);
        }
    }
}
