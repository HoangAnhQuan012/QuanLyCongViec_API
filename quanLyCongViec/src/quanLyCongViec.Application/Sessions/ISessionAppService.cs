using System.Threading.Tasks;
using Abp.Application.Services;
using quanLyCongViec.Sessions.Dto;

namespace quanLyCongViec.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
