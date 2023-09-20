using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using quanLyCongViec.DbEntities;
using quanLyCongViec.Global;
using quanLyCongViec.UnitsManagement.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quanLyCongViec.UnitsManagement
{
    public class UnitsManagementAppService : quanLyCongViecAppServiceBase
    {
        private readonly IRepository<Units> _unitsRepository;
        public UnitsManagementAppService(IRepository<Units> unitsRepository)
        {
            this._unitsRepository = unitsRepository;
        }

        public async Task<List<UnitsTreeTableForViewDto>> GetAllUnitsAsync(string input)
        {
            var list = await this._unitsRepository.GetAll().Select(e => new UnitsForViewDto
            {
                Units = e
            }).ToListAsync();

            List<UnitsTreeTableForViewDto> UnitsList = this.GetAllUnitsTreeTable(list, null);
            List<UnitsTreeTableForViewDto> result = new List<UnitsTreeTableForViewDto>();

            var filterText = GlobalFunction.RegexFormat(input ?? string.Empty).ToLower();
            if (!string.IsNullOrEmpty(filterText))
            {
                List<UnitsTreeTableForViewDto> filterItem = new List<UnitsTreeTableForViewDto>();
                foreach(var item in UnitsList)
                {
                    UnitsTreeTableForViewDto newItem = this.FilterItem(item, filterText);
                    if (newItem != null)
                    {
                        filterItem.Add(newItem);
                    }
                }
                result = filterItem;
            }
            else
            {
                result = UnitsList;
            }
            return await Task.FromResult(result);

        }

        public async Task<List<Units>> GetAllAsync()
        {
            return await this._unitsRepository.GetAll().ToListAsync();
        }

        public async Task CreateUnit(CreateUnitDto input)
        {
            try
            {
                var create = this.ObjectMapper.Map<Units>(input);
                await this._unitsRepository.InsertAndGetIdAsync(create);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<UnitsTreeTableForViewDto> GetAllUnitsTreeTable(List<UnitsForViewDto> UnitsList, int? Id)
        {
            return UnitsList.Where(w => w.Units.ParentUnitId == Id).OrderBy(e => e.Units.UnitName).Select(o => new UnitsTreeTableForViewDto
            {
                Data = o,
                Children = GetAllUnitsTreeTable(UnitsList, o.Units.Id),
                Expanded = true
            }).ToList();
        }

        private UnitsTreeTableForViewDto FilterItem(UnitsTreeTableForViewDto item, string filterText)
        {
            if (item.Data.Units.UnitName.ToLower().Contains(filterText))
            {
                return item;
            }
            else
            {
                if (item.Children != null)
                {
                    List<UnitsTreeTableForViewDto> children = new List<UnitsTreeTableForViewDto>();

                    item.Children.ForEach(child =>
                    {
                        var newChild = this.FilterItem(child, filterText);
                        if (newChild != null)
                        {
                            children.Add(newChild);
                        }
                    });

                    if (children.Count > 0)
                    {
                        var newItem = item;
                        newItem.Expanded = true;
                        newItem.Children = children;
                        return newItem;
                    }
                }
            }

            return null;
        }
    }
}
