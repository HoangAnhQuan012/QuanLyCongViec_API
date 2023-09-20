using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quanLyCongViec.Global.Dto
{
    public class TreeviewItemDto
    {
        public string Text { get; set; }

        public int? Value { get; set; }

        public bool? Collapsed { get; set; }

        public bool? Checked { get; set; }

        public bool? Disabled { get; set; }

        public List<TreeviewItemDto> Children { get; set; }
    }
}
