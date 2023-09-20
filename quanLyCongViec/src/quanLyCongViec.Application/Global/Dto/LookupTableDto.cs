using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quanLyCongViec.Global.Dto
{
    public class LookupTableDto : LookupTableDto<int>
    {
    }
    public class LookupTableDto<TPrimaryKey>
    {
        public TPrimaryKey Id { get; set; }
        public string DisplayName { get; set; }
    }
}
