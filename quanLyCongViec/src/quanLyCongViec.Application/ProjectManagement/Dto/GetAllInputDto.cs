using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;

namespace quanLyCongViec.ProjectManagement.Dto
{
    public class GetAllInputDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
    }
}
