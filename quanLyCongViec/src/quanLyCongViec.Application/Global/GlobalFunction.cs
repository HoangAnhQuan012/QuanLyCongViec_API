using Abp.Domain.Repositories;
using Abp.UI;
using quanLyCongViec.DbEntities;
using quanLyCongViec.Global.Dto;
using System;
using System.Collections.Generic;
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
    }
}
