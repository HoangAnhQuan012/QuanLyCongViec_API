using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using quanLyCongViec.Authorization.Users;
using quanLyCongViec.DbEntities;
using quanLyCongViec.Sprint_Module_Job.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quanLyCongViec.Sprint_Module_Job
{
    public class ModuleAppService : quanLyCongViecAppServiceBase
    {
        private readonly IRepository<Module> _moduleRepository;
        private readonly IRepository<User, long> _userRepository;
        public ModuleAppService(IRepository<Module> moduleRepository,
            IRepository<User, long> userRepository)
        {
            _moduleRepository = moduleRepository;
            _userRepository = userRepository;
        }

        public async Task CreateModuleAsync(CreateModuleInputDto input)
        {
            try
            {
                if (input == null)
                {
                    throw new UserFriendlyException("Input is null!");
                }
                var create = new Module
                {
                    ModuleName = input.ModuleName,
                    ProjectId = input.ProjectId,
                };
                if (input.ProjectManagerId == null || input.ProjectManagerId == 0)
                {
                    create.ProjectManagerId = (int)this.AbpSession.UserId;
                }
                await this._moduleRepository.InsertAsync(create);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> GetUserId()
        {
            var query = await this._userRepository.GetAll().Where(e => e.Id == this.AbpSession.UserId).Select(e => e.Name).FirstOrDefaultAsync();
            return query;
        }
    }
}
