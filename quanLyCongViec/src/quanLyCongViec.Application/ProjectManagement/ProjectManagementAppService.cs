using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using quanLyCongViec.Authorization.Users;
using quanLyCongViec.Data.Excel.Dtos;
using quanLyCongViec.DbEntities;
using quanLyCongViec.Global;
using quanLyCongViec.Global.Dto;
using quanLyCongViec.ProjectManagement.Dto;
using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly IAppFolder _appFolder;
        public ProjectManagementAppService(
            IRepository<Projects> projectRepository,
            IRepository<User, long> userRepository,
            IRepository<Units> unitsRepository,
            IRepository<ProjectAttachedFiles, long> attachedFilesRepository,
            IWebHostEnvironment env,
            IAppFolder appFolder)
        {
            this._projectRepository = projectRepository;
            this._userRepository = userRepository;
            this._unitRepository = unitsRepository;
            this._attachedFilesRepository = attachedFilesRepository;
            this._env = env;
            _appFolder = appFolder;
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
                            from u in this._userRepository.GetAll().Where(e => e.Id == o.ProjectManagerId)
                            select new ProjectsForViewDto()
                            {
                                Id = o.Id,
                                ProjectName = o.ProjectName,
                                Customer = o.Customer,
                                ProjectManagerId = u.Id,
                                ProjectManagerName = u.Name,
                                Status = o.Status,
                                StatusName = GlobalModel.ProjectStatusSorted[(int)o.Status],
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

        public async Task CreateOrEditProjectAsync (ProjectInputDto input)
        {
            if (input.Id == null || input.Id == 0)
            {
                foreach (var id in input.UserId)
                {
                    input.ProjectManagerName = GlobalFunction.RegexFormat(input.ProjectManagerName);
                    input.ProjectName = GlobalFunction.RegexFormat(input.ProjectName);
                    input.Customer = GlobalFunction.RegexFormat(input.Customer);
                    input.Note = GlobalFunction.RegexFormat(input.Note);

                    var create = new Projects
                    {
                        ProjectName = input.ProjectName,
                        Customer = input.Customer,
                        Status = (int)GlobalConst.ProjectStatus.NotStarted,
                        StartDate = input.StartDate,
                        EndDate = input.EndDate,
                        Note = input.Note,
                        UserId = id,
                    };

                    if (input.ProjectManagerId == null)
                    {
                        create.ProjectManagerId = await this._userRepository.GetAll().Where(e => e.Id == this.AbpSession.UserId).Select(e => e.Id).FirstOrDefaultAsync();
                    }
                    if (input.ProjectManagerName == null)
                    {
                        create.ProjectManagerName = await this._userRepository.GetAll().Where(e => e.Id == this.AbpSession.UserId).Select(e => e.Name).FirstOrDefaultAsync();
                    }

                    create.ProjectManagerId = (from user in this._userRepository.GetAll().Where(w => w.Id == this.AbpSession.UserId)
                                               select user.Id).FirstOrDefault();
                    create.ProjectManagerName = (from user in this._userRepository.GetAll().Where(w => w.Id == this.AbpSession.UserId)
                                                 select user.Name).FirstOrDefault();

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

                    await this._projectRepository.InsertAndGetIdAsync(create);
                }
            }
            else
            {
                var update = await this._projectRepository.FirstOrDefaultAsync(e => e.Id == input.Id);
                if (update == null)
                {
                    throw new UserFriendlyException("Input is null");
                }

                foreach (var user in input.UserId)
                {
                    update.ProjectName = input.ProjectName;
                    update.Customer = input.Customer;
                    update.ProjectManagerId = this._projectRepository.GetAll().Where(e => e.Id == input.Id).Select(w => w.ProjectManagerId).FirstOrDefault();
                    update.ProjectManagerName = this._projectRepository.GetAll().Where(e => e.Id == input.Id).Select(w => w.ProjectManagerName).FirstOrDefault();
                    update.Status = input.Status;
                    update.StartDate = input.StartDate.ToLocalTime();
                    update.EndDate = input.EndDate.ToLocalTime();
                    update.Note = input.Note;
                    update.UserId = user;

                    foreach(var item in input.ProjectAttachedFiles)
                    {
                        var file = new ProjectAttachedFiles
                        {
                            ProjectId = input.Id,
                            FileName = item.FileName,
                            FilePath = item.FilePath
                        };

                        await this._attachedFilesRepository.InsertAsync(file);
                    }

                    await this._projectRepository.UpdateAsync(update);
                }

            }
        }

        public async Task<FileDto> DownloadFileUpload(string linkFile)
        {
            if (string.IsNullOrEmpty(linkFile))
            {
                throw new UserFriendlyException("Link file is null");
            }

            var fileName = linkFile.Split(Path.DirectorySeparatorChar).Last();
            var path = this._appFolder.ProjectFileUploadFolder + linkFile.Replace(fileName, string.Empty);

            // _appFolders.DemoFileDownloadFolder : Thư mục chưa file mẫu cần tải
            // _appFolders.TempFileDownloadFolder : Không được sửa
            return await GlobalFunction.DownloadFileMau(fileName, path, this._appFolder.TempFileDownloadFolder);
        }

        public async Task<string> GetPMbyUserIdAsync()
        {
            var query = await this._userRepository.GetAll().Where(e => e.Id == this.AbpSession.UserId).Select(s => s.Name).FirstOrDefaultAsync();
            return query;
        }

        public async Task<string> GetStatusStart()
        {
              return GlobalModel.ProjectStatusSorted[(int)GlobalConst.ProjectStatus.NotStarted];
        }

        public async Task<ProjectsForViewDto> GetForEdit(int id)
        {
            List<ProjectAttachedFiles> projectAttachedFiles = new List<ProjectAttachedFiles>();
            var query = await this._projectRepository.GetAll().Where(e => e.Id == id).Select(s => new ProjectsForViewDto
            {
                Id = s.Id,
                ProjectManagerName = s.ProjectManagerName,
                ProjectName = s.ProjectName,
                Customer = s.Customer,
                Status = s.Status,
                StatusName = GlobalModel.ProjectStatusSorted[(int)s.Status],
                StartDate = s.StartDate,
                EndDate = s.EndDate,
                Note = s.Note,
            }).FirstOrDefaultAsync();

            return query;
        }

        public async Task<int> CheckAdmin()
        {
            var query = await this._userRepository.GetAll().Where(e => e.Id == this.AbpSession.UserId).Select(s => s.UnitId).FirstOrDefaultAsync();
            return query;
        }

        public async Task<List<LookupTableDto>> GetListUserDev()
        {
            var query = await this._userRepository.GetAll().Where(e => e.UnitId == 2).Select(s => new LookupTableDto
            {
                Id = (int)s.Id,
                DisplayName = s.Name,
            }).ToListAsync();
            return query;
        }
    }
}
