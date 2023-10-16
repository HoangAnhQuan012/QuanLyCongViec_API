using quanLyCongViec.DbEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quanLyCongViec.WorkReportManagement.Dtos
{
    public class CreateWorkReportInputDto
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int SprintId { get; set; }
        public int ModuleId { get; set; }
        public DateTime DeclarationDate { get; set; }
        public List<JobList> JobList { get; set; }
        public int Status { get; set; }
        public List<WorkReportAttachedFiles> AttachedFiles { get; set; }
    }

    public class JobList
    {
        public int JobId { get; set; }
        public int KindOfJob { get; set; }
        public int Type { get; set; }
        public bool OnSite { get; set; }
        public double Hours { get; set; }
        public string Note { get; set; }
    }
}
