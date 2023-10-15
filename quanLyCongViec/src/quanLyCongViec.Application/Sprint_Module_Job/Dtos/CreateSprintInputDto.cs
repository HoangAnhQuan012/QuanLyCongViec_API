using Abp.AutoMapper;
using quanLyCongViec.DbEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quanLyCongViec.SprintManagement.Dtos
{
    [AutoMap(typeof(Sprint))]
    public class CreateSprintInputDto
    {
        public string SprintName { get; set; }
        public int ProjectId { get; set; }
        public int ProjectManagerId { get; set; }
    }
}
