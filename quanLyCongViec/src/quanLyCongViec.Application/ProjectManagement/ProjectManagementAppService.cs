using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using quanLyCongViec.Authorization.Users;
using quanLyCongViec.DbEntities;
using quanLyCongViec.Global;
using quanLyCongViec.ProjectManagement.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace quanLyCongViec.ProjectManagement
{
    public class ProjectManagementAppService : quanLyCongViecAppServiceBase, IProjectManagementAppService
    {
        private readonly IRepository<Projects> _projectRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<Units> _unitRepository;
        private readonly IRepository<ProjectAttachedFiles, long> _attachedFilesRepository;
        private readonly IWebHostEnvironment _env;
        public ProjectManagementAppService(
            IRepository<Projects> projectRepository,
            IRepository<User, long> userRepository,
            IRepository<Units> unitsRepository,
            IRepository<ProjectAttachedFiles, long> attachedFilesRepository,
            IWebHostEnvironment env)
        {
            this._projectRepository = projectRepository;
            this._userRepository = userRepository;
            this._unitRepository = unitsRepository;
            this._attachedFilesRepository = attachedFilesRepository;
            this._env = env;
        }

        public async Task<PagedResultDto<ProjectsForViewDto>> GetAllProjectAsync(GetAllInputDto input)
        {
            try
            {
                var keyword = GlobalFunction.RegexFormat(input.Keyword);

                IQueryable<Projects> filter = null;

                filter = this._projectRepository.GetAll().WhereIf(!string.IsNullOrEmpty(keyword), e => e.ProjectManagerName.Contains(keyword)
                                                                  || e.ProjectName.Contains(keyword) || e.Customer.Contains(keyword)).AsQueryable();

                var query = from o in filter
                            from u in this._unitRepository.GetAll().Where(e => e.Id == o.ProjectManagerId)
                            select new ProjectsForViewDto()
                            {
                                Id = o.Id,
                                ProjectName = o.ProjectName,
                                Customer = o.Customer,
                                ProjectManagerId = u.Id,
                                ProjectManagerName = u.UnitName,
                                Status = o.Status,
                                StartDate = o.StartDate,
                                EndDate = o.EndDate,
                                Note = o.Note,
                                CreationTime = o.CreationTime,
                            };

                int totalCount = await query.CountAsync();
                var output = await query.OrderBy(input.Sorting ?? "CreationTime DESC").PageBy(input).ToListAsync();

                return new PagedResultDto<ProjectsForViewDto>(totalCount, output);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<long> CreateProjectAsync (ProjectInputDto input)
        {
            if (input == null)
            {
                throw new UserFriendlyException("Input is null");
            }

            input.ProjectManagerName = GlobalFunction.RegexFormat(input.ProjectManagerName);
            input.ProjectName = GlobalFunction.RegexFormat(input.ProjectName);
            input.Customer = GlobalFunction.RegexFormat(input.Customer);
            input.Note = GlobalFunction.RegexFormat(input.Note);

            var create = new Projects
            {
                ProjectName = input.ProjectName,
                Customer = input.Customer,
                ProjectManagerId = input.ProjectManagerId,
                ProjectManagerName = input.ProjectManagerName,
                Status = input.Status,
                StartDate = input.StartDate,
                EndDate = input.EndDate,
                Note = input.Note,
            };

            List<ProjectAttachedFiles> projectAttachedFiles = new List<ProjectAttachedFiles>();
            if (input.ProjectAttachedFiles != null)
            {
                projectAttachedFiles.AddRange(input.ProjectAttachedFiles.Select(e => new ProjectAttachedFiles
                {
                    FileName = e.FileName,
                    FilePath = e.FilePath,
                }));
            }

            create.ProjectAttachedFiles = projectAttachedFiles;

            var result = await this._projectRepository.InsertAndGetIdAsync(create);
            return result;
        }
    }
}
