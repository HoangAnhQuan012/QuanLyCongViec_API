using System.Threading.Tasks;
using quanLyCongViec.Configuration.Dto;

namespace quanLyCongViec.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}
