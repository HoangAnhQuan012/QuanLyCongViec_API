using Abp.Application.Services;
using quanLyCongViec.MultiTenancy.Dto;

namespace quanLyCongViec.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}

