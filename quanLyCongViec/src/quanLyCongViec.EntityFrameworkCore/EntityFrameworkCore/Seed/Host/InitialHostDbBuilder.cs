namespace quanLyCongViec.EntityFrameworkCore.Seed.Host
{
    public class InitialHostDbBuilder
    {
        private readonly quanLyCongViecDbContext _context;

        public InitialHostDbBuilder(quanLyCongViecDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            new DefaultEditionCreator(_context).Create();
            new DefaultLanguagesCreator(_context).Create();
            new HostRoleAndUserCreator(_context).Create();
            new DefaultSettingsCreator(_context).Create();

            _context.SaveChanges();
        }
    }
}
