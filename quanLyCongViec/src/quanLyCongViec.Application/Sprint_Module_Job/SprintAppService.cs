using Abp.Domain.Repositories;
using Abp.UI;
using quanLyCongViec.DbEntities;
using quanLyCongViec.SprintManagement.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quanLyCongViec.SprintManagement
{
    public class SprintAppService : quanLyCongViecAppServiceBase
    {
        private readonly IRepository<Sprint> _sprintRepository;
        public SprintAppService(
            IRepository<Sprint> sprintRepository
            )
        {
            _sprintRepository = sprintRepository;
        }

        public async Task CreateSprintAsync(CreateSprintInputDto input)
        {
            if (input == null)
            {
                throw new UserFriendlyException("Input is null!");
            }

            var create = this.ObjectMapper.Map<Sprint>(input);
            if (input.ProjectManagerId == 0 || input.ProjectManagerId == null)
            {
                create.ProjectManagerId = (int)this.AbpSession.UserId;
            }
            await this._sprintRepository.InsertAsync(create);
        }
    }
}
