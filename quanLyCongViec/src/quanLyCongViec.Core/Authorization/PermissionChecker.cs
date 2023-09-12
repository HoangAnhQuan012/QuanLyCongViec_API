using Abp.Authorization;
using quanLyCongViec.Authorization.Roles;
using quanLyCongViec.Authorization.Users;

namespace quanLyCongViec.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
