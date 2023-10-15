using Abp.Authorization;
using Microsoft.AspNetCore.Mvc;
using quanLyCongViec.Controllers;
using quanLyCongViec.Global;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quanLyCongViec
{
    [AbpAuthorize]
    [Route("api/[controller]/[action]")]
    public class UploadController : quanLyCongViecControllerBase
    {
        private readonly IAppFolder _appFolder;
        public UploadController(IAppFolder appFolder)
        {
            _appFolder = appFolder;
        }

        [HttpPost]
        public async Task<List<string>> ProjectUpload()
        {
            string fileFolderPath = Path.Combine(_appFolder.ProjectFileUploadFolder + Path.DirectorySeparatorChar + string.Format("{0:yyyyMMdd_hhmmss}", DateTime.Now));
            return await Upload(fileFolderPath);
        }

        [HttpPost]
        public async Task<List<string>> WorkReportUpload()
        {
            string fileFolderPath = Path.Combine(_appFolder.WorkReportFileUploadFolder + Path.DirectorySeparatorChar + string.Format("{0:yyyyMMdd_hhmmss}", DateTime.Now));
            return await Upload(fileFolderPath);
        }


        /// <summary>
        /// Không sửa
        /// </summary>
        /// <param name="fileFolderPath"></param>
        /// <returns></returns>
        private async Task<List<string>> Upload(string fileFolderPath)
        {
            List<string> result = new List<string>();

            List<string> allowedExtensions = new List<string> { ".doc", ".docx", ".xls", ".xlsx", ".pdf", ".png", ".jpg" };
            // Náº¿u khÃ´ng cÃ³ file 
            if (Request.Form.Files == null || Request.Form.Files.Count <= 0)
            {
                return result;
            }

            var checkFile = false;
            foreach (var file in Request.Form.Files)
            {
                // Lấy phần mở rộng của tên tệp
                var fileExtension = Path.GetExtension(file.FileName).ToLower();

                if (allowedExtensions.Contains(fileExtension))
                {
                    result.Add(GlobalFunction.SaveFile(fileFolderPath, file));
                }
                else
                {
                    checkFile = true;
                }
            }

            this.Assert(checkFile, "Invalid file!");
            return await Task.FromResult(result);
        }
    }
}
