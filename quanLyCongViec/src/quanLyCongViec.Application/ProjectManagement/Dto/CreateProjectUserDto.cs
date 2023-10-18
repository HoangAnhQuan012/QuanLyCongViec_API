using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quanLyCongViec.ProjectManagement.Dto
{
    public class CreateProjectUserDto
    {
        public int ProjectId { get; set; }
        public List<ProjectUserDto> ProjectUsers { get; set; }
    }
}
