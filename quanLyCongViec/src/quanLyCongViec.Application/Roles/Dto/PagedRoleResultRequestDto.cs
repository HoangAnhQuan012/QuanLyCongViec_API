using Abp.Application.Services.Dto;

namespace quanLyCongViec.Roles.Dto
{
    public class PagedRoleResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
    }
}

