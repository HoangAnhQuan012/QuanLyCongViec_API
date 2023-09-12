using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using quanLyCongViec.Authorization.Roles;
using quanLyCongViec.Authorization.Users;
using quanLyCongViec.MultiTenancy;

namespace quanLyCongViec.EntityFrameworkCore
{
    public class quanLyCongViecDbContext : AbpZeroDbContext<Tenant, Role, User, quanLyCongViecDbContext>
    {
        /* Define a DbSet for each entity of the application */
        
        public quanLyCongViecDbContext(DbContextOptions<quanLyCongViecDbContext> options)
            : base(options)
        {
        }
    }
}
