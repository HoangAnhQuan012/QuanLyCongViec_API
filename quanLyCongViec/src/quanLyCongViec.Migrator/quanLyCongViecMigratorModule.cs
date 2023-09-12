using Microsoft.Extensions.Configuration;
using Castle.MicroKernel.Registration;
using Abp.Events.Bus;
using Abp.Modules;
using Abp.Reflection.Extensions;
using quanLyCongViec.Configuration;
using quanLyCongViec.EntityFrameworkCore;
using quanLyCongViec.Migrator.DependencyInjection;

namespace quanLyCongViec.Migrator
{
    [DependsOn(typeof(quanLyCongViecEntityFrameworkModule))]
    public class quanLyCongViecMigratorModule : AbpModule
    {
        private readonly IConfigurationRoot _appConfiguration;

        public quanLyCongViecMigratorModule(quanLyCongViecEntityFrameworkModule abpProjectNameEntityFrameworkModule)
        {
            abpProjectNameEntityFrameworkModule.SkipDbSeed = true;

            _appConfiguration = AppConfigurations.Get(
                typeof(quanLyCongViecMigratorModule).GetAssembly().GetDirectoryPathOrNull()
            );
        }

        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(
                quanLyCongViecConsts.ConnectionStringName
            );

            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
            Configuration.ReplaceService(
                typeof(IEventBus), 
                () => IocManager.IocContainer.Register(
                    Component.For<IEventBus>().Instance(NullEventBus.Instance)
                )
            );
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(quanLyCongViecMigratorModule).GetAssembly());
            ServiceCollectionRegistrar.Register(IocManager);
        }
    }
}
