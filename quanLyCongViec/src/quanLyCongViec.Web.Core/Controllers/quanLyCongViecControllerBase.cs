using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace quanLyCongViec.Controllers
{
    public abstract class quanLyCongViecControllerBase: AbpController
    {
        protected quanLyCongViecControllerBase()
        {
            LocalizationSourceName = quanLyCongViecConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
