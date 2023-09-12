using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using quanLyCongViec.Authorization;

namespace quanLyCongViec
{
    [DependsOn(
        typeof(quanLyCongViecCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class quanLyCongViecApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<quanLyCongViecAuthorizationProvider>();
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(quanLyCongViecApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddMaps(thisAssembly)
            );
        }
    }
}
