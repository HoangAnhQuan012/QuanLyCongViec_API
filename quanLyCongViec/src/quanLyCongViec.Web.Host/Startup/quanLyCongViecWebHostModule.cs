using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using quanLyCongViec.Configuration;

namespace quanLyCongViec.Web.Host.Startup
{
    [DependsOn(
       typeof(quanLyCongViecWebCoreModule))]
    public class quanLyCongViecWebHostModule: AbpModule
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public quanLyCongViecWebHostModule(IWebHostEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(quanLyCongViecWebHostModule).GetAssembly());
        }
    }
}
