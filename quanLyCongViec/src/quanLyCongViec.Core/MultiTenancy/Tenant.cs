using Abp.MultiTenancy;
using quanLyCongViec.Authorization.Users;

namespace quanLyCongViec.MultiTenancy
{
    public class Tenant : AbpTenant<User>
    {
        public Tenant()
        {            
        }

        public Tenant(string tenancyName, string name)
            : base(tenancyName, name)
        {
        }
    }
}
