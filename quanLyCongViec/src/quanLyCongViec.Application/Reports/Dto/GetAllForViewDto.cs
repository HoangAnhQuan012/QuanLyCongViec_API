using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quanLyCongViec.Reports.Dto
{
    public class GetAllForViewDto
    {
        public List<string> ListUserName { get; set; }
        public List<int> DaysLogged { get; set; }
        public List<double> HoursWorked { get; set; }
    }
}
