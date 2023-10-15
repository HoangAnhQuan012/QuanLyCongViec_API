using System;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Abp.AspNetCore;
using Abp.AspNetCore.Configuration;
using Abp.AspNetCore.SignalR;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Zero.Configuration;
using quanLyCongViec.Authentication.JwtBearer;
using quanLyCongViec.Configuration;
using quanLyCongViec.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System.IO;
using Abp.IO;

namespace quanLyCongViec
{
    [DependsOn(
         typeof(quanLyCongViecApplicationModule),
         typeof(quanLyCongViecEntityFrameworkModule),
         typeof(AbpAspNetCoreModule)
        ,typeof(AbpAspNetCoreSignalRModule)
     )]
    public class quanLyCongViecWebCoreModule : AbpModule
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public quanLyCongViecWebCoreModule(IWebHostEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(
                quanLyCongViecConsts.ConnectionStringName
            );

            // Use database for language management
            Configuration.Modules.Zero().LanguageManagement.EnableDbLocalization();

            Configuration.Modules.AbpAspNetCore()
                 .CreateControllersForAppServices(
                     typeof(quanLyCongViecApplicationModule).GetAssembly()
                 );

            ConfigureTokenAuth();
        }

        private void ConfigureTokenAuth()
        {
            IocManager.Register<TokenAuthConfiguration>();
            var tokenAuthConfig = IocManager.Resolve<TokenAuthConfiguration>();

            tokenAuthConfig.SecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_appConfiguration["Authentication:JwtBearer:SecurityKey"]));
            tokenAuthConfig.Issuer = _appConfiguration["Authentication:JwtBearer:Issuer"];
            tokenAuthConfig.Audience = _appConfiguration["Authentication:JwtBearer:Audience"];
            tokenAuthConfig.SigningCredentials = new SigningCredentials(tokenAuthConfig.SecurityKey, SecurityAlgorithms.HmacSha256);
            tokenAuthConfig.Expiration = TimeSpan.FromDays(1);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(quanLyCongViecWebCoreModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            IocManager.Resolve<ApplicationPartManager>()
                .AddApplicationPartsIfNotAddedBefore(typeof(quanLyCongViecWebCoreModule).Assembly);
            SetAppFolders();
        }

        private void SetAppFolders()
        {
            var appFolders = IocManager.Resolve<AppFolder>();

            appFolders.TempFileDownloadFolder = Path.Combine(_env.WebRootPath, $"Temp{Path.DirectorySeparatorChar}Downloads");

            appFolders.ProjectFileUploadFolder = Path.Combine(_env.WebRootPath, $"Upload{Path.DirectorySeparatorChar}Project");
            appFolders.ProjectFileDownloadFolder = Path.Combine(_env.WebRootPath, $"Temp{Path.DirectorySeparatorChar}Downloads{Path.DirectorySeparatorChar}Imports{Path.DirectorySeparatorChar}Project");

            appFolders.WorkReportFileUploadFolder = Path.Combine(_env.WebRootPath, $"Upload{Path.DirectorySeparatorChar}WorkReport");
            appFolders.WorkReportFileDownloadFolder = Path.Combine(_env.WebRootPath, $"Temp{Path.DirectorySeparatorChar}Downloads{Path.DirectorySeparatorChar}Imports{Path.DirectorySeparatorChar}WorkReport");

            try
            {
                DirectoryHelper.CreateIfNotExists(appFolders.TempFileDownloadFolder);
                DirectoryHelper.CreateIfNotExists(appFolders.ProjectFileDownloadFolder);
                DirectoryHelper.CreateIfNotExists(appFolders.ProjectFileUploadFolder);
                DirectoryHelper.CreateIfNotExists(appFolders.WorkReportFileDownloadFolder);
                DirectoryHelper.CreateIfNotExists(appFolders.WorkReportFileUploadFolder);
            }
            catch {}
        }
    }
}
