using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.AspNetCore.Hosting;
using quanLyCongViec.Authorization.Users;
using quanLyCongViec.DbEntities;
using quanLyCongViec.Global;
using quanLyCongViec.ProjectManagement.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quanLyCongViec.ProjectManagement
{
    public class ProjectManagementAppService : quanLyCongViecAppServiceBase, IProjectManagementAppService
    {
        private readonly IRepository<Projects> _projectRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<Units> _staffsRepository;
        private readonly IRepository<ProjectAttachedFiles, long> _attachedFilesRepository;
        private readonly IWebHostEnvironment _env;
        public ProjectManagementAppService(
            IRepository<Projects> projectRepository,
            IRepository<User, long> userRepository,
            IRepository<Units> staffsRepository,
            IRepository<ProjectAttachedFiles, long> attachedFilesRepository,
            IWebHostEnvironment env)
        {
            this._projectRepository = projectRepository;
            this._userRepository = userRepository;
            this._staffsRepository = staffsRepository;
            this._attachedFilesRepository = attachedFilesRepository;
            this._env = env;
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
                TenantId = input.Id,
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
