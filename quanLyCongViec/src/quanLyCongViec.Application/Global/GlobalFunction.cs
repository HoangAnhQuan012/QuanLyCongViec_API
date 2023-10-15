using Abp.Domain.Repositories;
using Abp.IO;
using Abp.IO.Extensions;
using Abp.UI;
using Microsoft.AspNetCore.Http;
using quanLyCongViec.Data.Excel.Dtos;
using quanLyCongViec.DbEntities;
using quanLyCongViec.Global.Dto;
using quanLyCongViec.Net.MimeTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace quanLyCongViec.Global
{
    public static class GlobalFunction
    {
        /// <summary>
        /// Check parameter.
        /// </summary>
        /// <param name="obj">object.</param>
        /// <param name="condition">condition.</param>
        /// <param name="message">message.</param>
        /// 
        public static void Assert(this object obj, bool condition, string message)
        {
            if (condition)
            {
                throw new UserFriendlyException(message);
            }
        }
        public static string RegexFormat(string input)
        {
            if (input != null)
            {
                return Regex.Replace(input, @"\s+", " ").Trim();
            }
            else
                return input;
        }

        public static async Task<List<TreeviewItemDto>> GetAllUnitsTreeAsync(IRepository<Units> unitsRepository)
        {
            var listParent = unitsRepository.GetAllList();
            var listTong = GetUnitsChildren(listParent, null);
            return await Task.FromResult(listTong);
        }

        private static List<TreeviewItemDto> GetUnitsChildren(List<Units> list, int? id)
        {
            return list.Where(w => w.ParentUnitId == id).Select(w => new TreeviewItemDto
            {
                Text = w.UnitName,
                Value = w.Id,
                Checked = false,
                Children = GetUnitsChildren(list, w.Id),
            }).ToList();
        }

        /// <summary>
        /// Hàm lưu file
        /// </summary>
        /// <param name="FolderPath">Đường dẫn lưu file trên server</param>
        /// <param name="ImportFile">File</param>
        /// <returns>Đường dẫn trỏ tới file trên server</returns>
        public static string SaveFile(string FolderPath, IFormFile ImportFile)
        {
            byte[] fileBytes;
            using (var stream = ImportFile.OpenReadStream())
            {
                fileBytes = stream.GetAllBytes();
            }

            string uploadFileName = string.Format("{0:yyyyMMdd_hhmmss}_", DateTime.Now) + ImportFile.FileName;

            // Set full path to upload file
            DirectoryHelper.CreateIfNotExists(FolderPath);
            string uploadFilePath = Path.Combine(FolderPath, uploadFileName);
            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }
            // Save new file
            File.WriteAllBytes(uploadFilePath, fileBytes);

            return uploadFilePath.Substring(uploadFilePath.IndexOf("Upload"));
        }

        /// <summary>
        /// Tải file mẫu
        /// </summary>
        /// <param name="FileName">Tên file cần tải</param>
        /// <param name="pathFileDownload">Thư mục chứa file cần tải</param>
        /// <param name="pathFileToken">Thư mục chứa token down ( Không được sửa )</param>
        /// <returns></returns>
        public static Task<FileDto> DownloadFileMau(string FileName, string pathFileDownload, string pathFileToken)
        {
            List<string> allowedExtensions = new List<string>
            {
                ".doc", ".docx", ".xls", ".xlsx", ".pdf", ".png", ".jpg"
            };

            string fileExtension = Path.GetExtension(FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
            {
                throw new Exception("Tệp không hợp lệ!");
            }

            var result = new FileDto(FileName, MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet);
            string SourceFile = Path.Combine(pathFileDownload, FileName);
            string DestinationFile = Path.Combine(pathFileToken, result.FileToken);
            File.Copy(SourceFile, DestinationFile, true);
            return Task.FromResult(result);
        }
    }
}
