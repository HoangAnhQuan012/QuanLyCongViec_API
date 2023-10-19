﻿using Abp.Authorization;
using Abp.Localization;
using Abp.MultiTenancy;

namespace quanLyCongViec.Authorization
{
    public class quanLyCongViecAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            context.CreatePermission(PermissionNames.Pages_Users, L("Users"));
            context.CreatePermission(PermissionNames.Pages_Users_Activation, L("UsersActivation"));
            context.CreatePermission(PermissionNames.Pages_Roles, L("Roles"));
            context.CreatePermission(PermissionNames.Pages_Tenants, L("Tenants"), multiTenancySides: MultiTenancySides.Host);
            context.CreatePermission(PermissionNames.Pages_ProjectManager, L("ProjectManager"));
            context.CreatePermission(PermissionNames.Pages_ProjectManager_Create, L("ProjectManagerCreate"));
            context.CreatePermission(PermissionNames.Pages_Reports, L("Reports"));
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, quanLyCongViecConsts.LocalizationSourceName);
        }
    }
}
