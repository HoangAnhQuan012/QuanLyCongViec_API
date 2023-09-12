using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Runtime.Session;
using quanLyCongViec.Configuration.Dto;

namespace quanLyCongViec.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : quanLyCongViecAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
    }
}
