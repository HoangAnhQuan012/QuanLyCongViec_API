using quanLyCongViec.DbEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quanLyCongViec.ProjectManagement.Dto
{
    public class ProjectInputDto
    {
        public int Id { get; set; }
        public string ProjectName { get; set; }
        public string Customer { get; set; }
        public long ProjectManagerId { get; set; }
        public string ProjectManagerName { get; set; }
        public int Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Note { get; set; }
        public List<ProjectUser> ProjectUsers { get; set; }
        public List<ProjectAttachedFiles> ProjectAttachedFiles { get; set; }
    }
}
