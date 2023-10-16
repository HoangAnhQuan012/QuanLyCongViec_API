using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quanLyCongViec.WorkReportManagement.Dtos
{
    public class GetAllWorkReportForViewDto
    {
        public int Id { get; set; }
        public DateTime DeclarationDate { get; set; }
        public string SprineName { get; set; }
        public string ModuleName { get; set; }
        public double Hours { get; set; }
        public string Status { get; set; }
        public int StatusId { get; set; }
        public List<GetAllDetails> GetReportDetails { get; set; }
    }
}
