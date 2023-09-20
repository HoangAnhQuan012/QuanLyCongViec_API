using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using quanLyCongViec.Authorization.Users;
using quanLyCongViec.DbEntities;
using quanLyCongViec.Global.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quanLyCongViec.Global
{
    public class LookupTableAppService : quanLyCongViecAppServiceBase
    {
        private readonly IRepository<Units> _unitsRepository;
        private readonly IRepository<User, long> _userRepository;
        public LookupTableAppService(
            IRepository<Units> unitsRepository,
            IRepository<User, long> userRepository)
        {
            this._unitsRepository = unitsRepository;
            this._userRepository = userRepository;
        }

        public async Task<List<TreeviewItemDto>> GetAllUnitsTreeViewAsync()
        {
            return await GlobalFunction.GetAllUnitsTreeAsync(this._unitsRepository);
        }

        public async Task<List<LookupTableDto>> GetAllUnitsLookupTableAsync()
        {
            var result = await this._unitsRepository.GetAll().Select(e => new LookupTableDto() { Id = e.Id, DisplayName = e.UnitName }).ToListAsync();
            return await Task.FromResult(result);
        }

        public async Task<string> GetUnitNameByUserIdAsync()
        {
            string unitName = await (from user in this._userRepository.GetAll().Where(w => w.Id == this.AbpSession.UserId)
                                  from unit in this._unitsRepository.GetAll().Where(w => w.Id == user.UnitId)
                                  select unit.UnitName).FirstOrDefaultAsync();
            return unitName;
        }

        public async Task<string> GetUserLogIn()
        {
            var query = await this._userRepository.GetAll().Where(w => w.Id == this.AbpSession.UserId).Select(s => s.Name).FirstOrDefaultAsync();
            return query;
        }
    }
}
