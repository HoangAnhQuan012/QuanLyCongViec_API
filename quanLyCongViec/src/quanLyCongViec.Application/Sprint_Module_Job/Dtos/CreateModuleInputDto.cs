using Abp.AutoMapper;
using quanLyCongViec.DbEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quanLyCongViec.Sprint_Module_Job.Dtos
{
    public class CreateModuleInputDto
    {
        public string ModuleName { get; set; }
        public int ProjectId { get; set; }
        public int ProjectManagerId { get; set; }
    }
}
