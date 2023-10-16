using quanLyCongViec.DbEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quanLyCongViec.WorkReportManagement.Dtos
{
    public class GetAllDetails
    {
        public string JobName { get; set; }
        public double Hours { get; set; }
        public string KindOfJobName { get; set; }
        public string TypeName { get; set; }
        public List<WorkReportAttachedFiles> WorkReportAttachedFiles { get; set; }
    }
}
