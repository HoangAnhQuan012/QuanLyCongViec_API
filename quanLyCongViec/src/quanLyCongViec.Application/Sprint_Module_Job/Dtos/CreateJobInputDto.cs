using Abp.AutoMapper;
using quanLyCongViec.DbEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quanLyCongViec.Sprint_Module_Job.Dtos
{
    [AutoMap(typeof(Job))]
    public class CreateJobInputDto
    {
        public string JobName { get; set; }
        public int SprintId { get; set; }
    }
}
