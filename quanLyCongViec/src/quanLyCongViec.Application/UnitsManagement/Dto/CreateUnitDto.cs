using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using quanLyCongViec.DbEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quanLyCongViec.UnitsManagement.Dto
{
    [AutoMap(typeof(Units))]
    public class CreateUnitDto : EntityDto<int>
    {
        public string UnitName { get; set; }
        public int? ParentUnitId { get; set; }
        public string Note { get; set; }
    }
}
