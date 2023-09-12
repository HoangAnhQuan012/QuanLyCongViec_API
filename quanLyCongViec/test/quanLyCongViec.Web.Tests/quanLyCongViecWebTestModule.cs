using Abp.AspNetCore;
using Abp.AspNetCore.TestBase;
using Abp.Modules;
using Abp.Reflection.Extensions;
using quanLyCongViec.EntityFrameworkCore;
using quanLyCongViec.Web.Startup;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace quanLyCongViec.Web.Tests
{
    [DependsOn(
        typeof(quanLyCongViecWebMvcModule),
        typeof(AbpAspNetCoreTestBaseModule)
    )]
    public class quanLyCongViecWebTestModule : AbpModule
    {
        public quanLyCongViecWebTestModule(quanLyCongViecEntityFrameworkModule abpProjectNameEntityFrameworkModule)
        {
            abpProjectNameEntityFrameworkModule.SkipDbContextRegistration = true;
        } 
        
        public override void PreInitialize()
        {
            Configuration.UnitOfWork.IsTransactional = false; //EF Core InMemory DB does not support transactions.
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(quanLyCongViecWebTestModule).GetAssembly());
        }
        
        public override void PostInitialize()
        {
            IocManager.Resolve<ApplicationPartManager>()
                .AddApplicationPartsIfNotAddedBefore(typeof(quanLyCongViecWebMvcModule).Assembly);
        }
    }
}