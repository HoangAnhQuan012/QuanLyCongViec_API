using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace quanLyCongViec.EntityFrameworkCore
{
    public static class quanLyCongViecDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<quanLyCongViecDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<quanLyCongViecDbContext> builder, DbConnection connection)
        {
            builder.UseSqlServer(connection);
        }
    }
}
