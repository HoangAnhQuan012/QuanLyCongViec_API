using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quanLyCongViec.UnitsManagement.Dto
{
    public class UnitsTreeTableForViewDto
    {
        public UnitsForViewDto Data { get; set; }
        public List<UnitsTreeTableForViewDto> Children { get; set; }
        public bool Expanded { get; set; }
    }
}
