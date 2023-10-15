using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using quanLyCongViec.Authorization.Roles;
using quanLyCongViec.Authorization.Users;
using quanLyCongViec.MultiTenancy;
using quanLyCongViec.DbEntities;

namespace quanLyCongViec.EntityFrameworkCore
{
    public class quanLyCongViecDbContext : AbpZeroDbContext<Tenant, Role, User, quanLyCongViecDbContext>
    {
        /* Define a DbSet for each entity of the application */
        public DbSet<Projects> Projects { get; set; }
        public DbSet<ProjectAttachedFiles> ProjectAttachedFiles { get; set; }
        public DbSet<Units> Staffs { get; set; }
        public DbSet<Job> Job { get; set; }
        public DbSet<Module> Module { get; set; }
        public DbSet<Sprint> Sprint { get; set; }
        public DbSet<WorkReport> WorkReport { get; set; }
        public DbSet<WorkReportAttachedFiles> WorkReportAttachedFiles { get; set; }
        
        public quanLyCongViecDbContext(DbContextOptions<quanLyCongViecDbContext> options)
            : base(options)
        {
        }
    }
}
