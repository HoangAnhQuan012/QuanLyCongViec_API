using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using quanLyCongViec.DbEntities;
using quanLyCongViec.Global.Dto;
using quanLyCongViec.Sprint_Module_Job.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quanLyCongViec.Sprint_Module_Job
{
    public class JobAppService : quanLyCongViecAppServiceBase
    {
        private readonly IRepository<Job> _jobRepository;
        private readonly IRepository<Sprint> _sprintRepository;
        public JobAppService(
            IRepository<Job> jobRepository,
            IRepository<Sprint> sprintRepository
            )
        {
            _jobRepository = jobRepository;
            _sprintRepository = sprintRepository;
        }

        public async Task CreateJobAsync(CreateJobInputDto input)
        {
            if (input == null)
            {
                throw new UserFriendlyException("Input is null!");
            }

            var create = this.ObjectMapper.Map<Job>(input);
            create.ProjectManagerId = (int)this.AbpSession.UserId;
            await this._jobRepository.InsertAsync(create);
        }

        public async Task<List<LookupTableDto>> GetAllSprint(int projectId)
        {
            var query = await this._sprintRepository.GetAll().Where(w => w.ProjectId == projectId).Select(e => new LookupTableDto
            {
                Id = e.Id,
                DisplayName = e.SprintName
            }).ToListAsync();
            return query;
        }
    }
}
