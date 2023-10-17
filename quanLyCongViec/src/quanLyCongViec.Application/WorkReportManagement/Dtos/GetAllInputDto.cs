using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quanLyCongViec.WorkReportManagement.Dtos
{
    public class GetAllInputDto : PagedResultRequestDto
    {
        public int ProjectId { get; set; }
    }
}
