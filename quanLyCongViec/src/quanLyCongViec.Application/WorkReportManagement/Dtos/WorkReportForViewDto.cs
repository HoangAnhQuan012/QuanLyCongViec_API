using quanLyCongViec.DbEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quanLyCongViec.WorkReportManagement.Dtos
{
    public class WorkReportForViewDto
    {
        public int Id { get; set; }
        public string SpintName { get; set; }
        public string ModuleName { get; set; }
        public DateTime DeclarationDate { get; set; }
        public string JobName { get; set; }
        public string KindOfJobName { get; set; }
        public string TypeName { get; set; }
        public bool OnSite { get; set; }
        public double Hours { get; set; }
        public string Note { get; set; }
        public int StatusId { get; set; }
        public List<WorkReportAttachedFiles> ListFile { get; set; }
    }
}
