using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using quanLyCongViec.Authorization.Users;
using quanLyCongViec.Data.Excel.Dtos;
using quanLyCongViec.DbEntities;
using quanLyCongViec.Global;
using quanLyCongViec.Global.Dto;
using quanLyCongViec.WorkReportManagement.Dtos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static quanLyCongViec.Global.GlobalConst;

namespace quanLyCongViec.WorkReportManagement
{
    public class WorkReportAppService : quanLyCongViecAppServiceBase
    {
        private readonly IRepository<WorkReport> _workReportRepository;
        private readonly IRepository<WorkReportAttachedFiles, long> _workReportAttachedFilesRepository;
        private readonly IRepository<Sprint> _sprintRepository;
        private readonly IAppFolder _appFolder;
        private readonly IRepository<Module> _moduleRepository;
        private readonly IRepository<Job> _jobRepository;
        private readonly IRepository<ProjectUser, int> _projectUserRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<Units> _UnitsRepository;
        public WorkReportAppService(
                IRepository<WorkReport> workReportRepository,
                IRepository<WorkReportAttachedFiles, long> workReportAttachedFilesRepository,
                IAppFolder appFolder,
                IRepository<Sprint> sprint,
                IRepository<Module> moduleRepository,
                IRepository<Job> jobRepository,
                IRepository<ProjectUser, int> projectUserRepository,
                IRepository<User, long> userRepository,
                IRepository<Units> UnitsRepository
            )
        {
            _workReportRepository = workReportRepository;
            _workReportAttachedFilesRepository = workReportAttachedFilesRepository;
            _appFolder = appFolder;
            _sprintRepository = sprint;
            _moduleRepository = moduleRepository;
            _jobRepository = jobRepository;
            _projectUserRepository = projectUserRepository;
            _userRepository = userRepository;
            _UnitsRepository = UnitsRepository;
        }

        public async Task<PagedResultDto<GetAllWorkReportForViewDto>> GetAllWorkReport(GetAllInputDto input)
        {
            try
            {
                var userId = this.AbpSession.UserId;
                var getUnitId = await this._userRepository.GetAll().Where(e => e.Id == userId).Select(e => e.UnitId).FirstOrDefaultAsync();
                var isManager = this._UnitsRepository.GetAll().Where(e => e.Id == getUnitId).Select(e => e.ParentUnitId).FirstOrDefault();

                var getProjectIds = await this._workReportRepository.GetAll().Select(e => e.ProjectId).ToListAsync();
                var staffId = await this._projectUserRepository.GetAll().Where(e => e.UserId == userId).Select(e => e.UserId).FirstOrDefaultAsync();

                var filter = this._workReportRepository.GetAll().Where(e => e.ProjectId == input.ProjectId)
                                                                .WhereIf(isManager == null, e => getProjectIds.Contains(e.ProjectId))
                                                                .WhereIf(isManager != null, e => staffId == e.CreatorUserId);

                var result = from report in filter
                             from sprint in this._sprintRepository.GetAll().Where(e => e.Id == report.SprintId)
                             from module in this._moduleRepository.GetAll().Where(e => e.Id == report.ModuleId)
                             from user in this._userRepository.GetAll().Where(e => e.Id == report.CreatorUserId)
                             //from job in this._jobRepository.GetAll().Where(e => e.Id == report.JobId)
                             select new GetAllWorkReportForViewDto()
                             {
                                 Id = report.Id,
                                 DeclarationDate = report.DeclarationDate,
                                 SprineName = sprint.SprintName,
                                 ModuleName = module.ModuleName,
                                 Hours = report.Hours,
                                 Status = GlobalModel.WorkReportStatus[report.Status],
                                 StatusId = report.Status,
                                 CreationTime = report.CreationTime,
                                 UserName = user.UserName,
                                 //JobName = job.JobName,
                                 //KindOfJobName = GlobalModel.KindOfJob[report.KindOfJob],
                                 //TypeName = GlobalModel.Type[report.Type],
                                 //OnSite = report.OnSite,
                                 //Note = report.Note,
                                 GetReportDetails = (from job in this._jobRepository.GetAll().Where(e => e.Id == report.JobId)
                                                     from dinhKem in this._workReportAttachedFilesRepository.GetAll().Where(e => e.WorkReportId == report.Id).DefaultIfEmpty()
                                                     select new GetAllDetails
                                                     {
                                                         JobName = job.JobName,
                                                         KindOfJobName = GlobalModel.KindOfJob[report.KindOfJob],
                                                         TypeName = GlobalModel.Type[report.Type],
                                                         Hours = report.Hours,
                                                         WorkReportAttachedFiles = (from file in this._workReportAttachedFilesRepository.GetAll().Where(e => e.WorkReportId == report.Id)
                                                                                    select new WorkReportAttachedFiles
                                                                                    {
                                                                                        FileName = file.FileName,
                                                                                        FilePath = file.FilePath
                                                                                    }).ToList()
                                                     }).ToList()
                             };

                int totalCount = await result.CountAsync();
                var output = await result.OrderByDescending(e => e.CreationTime).PageBy(input).ToListAsync();

                return new PagedResultDto<GetAllWorkReportForViewDto>(totalCount, output);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<WorkReportForViewDto> GetForEdit (int id)
        {
            try
            {
                List<WorkReportAttachedFiles> workReportAttachedFiles = new List<WorkReportAttachedFiles>();
                var query = await (from report in this._workReportRepository.GetAll().Where(e => e.Id == id)
                                   from sprint in this._sprintRepository.GetAll().Where(e => e.Id == report.SprintId)
                                   from module in this._moduleRepository.GetAll().Where(e => e.Id == report.ModuleId)
                                   from job in this._jobRepository.GetAll().Where(e => e.Id == report.JobId)
                                   select new WorkReportForViewDto
                                   {
                                       Id = id,
                                       SpintName = sprint.SprintName,
                                       ModuleName = module.ModuleName,
                                       DeclarationDate = report.DeclarationDate,
                                       JobName = job.JobName,
                                       KindOfJobName = GlobalModel.KindOfJob[report.KindOfJob],
                                       TypeName = GlobalModel.Type[report.Type],
                                       OnSite = report.OnSite,
                                       Hours = report.Hours,
                                       Note = report.Note,
                                       StatusId = report.Status
                                   }).FirstOrDefaultAsync();
                var listDinhKem = await this._workReportAttachedFilesRepository.GetAll().Where(e => e.WorkReportId == id).ToListAsync();

                if (listDinhKem.Count > 0)
                {
                    foreach (var item in listDinhKem)
                    {
                        workReportAttachedFiles.Add(item);
                    }
                }

                query.ListFile = workReportAttachedFiles;

                return query;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> UpdateWorkReportStatus(UpdateReportStatusInput input)
        {
            var update = this._workReportRepository.GetAll().Where(e => e.Id == input.Id).FirstOrDefault();

            if (update != null)
            {
                if (input.Status == (int)WorkReportStatus.WaitForAppoval)
                {
                    update.Status = (int)WorkReportStatus.Approved;
                }
                else if (input.Status == (int)WorkReportStatus.Approved)
                {
                    update.Status = (int)WorkReportStatus.Rejected;
                }
            }
            else
            {
                return "Data not found";
            }

            return "Update success";
        }
        public async Task CreateOrEditWorkReportAsync(CreateWorkReportInputDto input)
        {
            try
            {
                if (input == null)
                {
                    throw new UserFriendlyException("Input is null");
                }

                if (input.Id == 0 || input.Id == null)
                {
                    var create = new WorkReport
                    {
                        ProjectId = input.ProjectId,
                        SprintId = input.SprintId,
                        ModuleId = input.ModuleId,
                        DeclarationDate = input.DeclarationDate.ToLocalTime(),
                        Status = input.Status,
                        JobId = input.JobId,
                        KindOfJob = input.KindOfJob,
                        Type = input.Type,
                        OnSite = input.OnSite,
                        Hours = input.Hours,
                        Note = input.Note
                    };

                    //foreach (var item in input.JobList)
                    //{
                    //    create.JobId = item.JobId;
                    //    create.KindOfJob = item.KindOfJob;
                    //    create.Type = item.Type;
                    //    create.OnSite = item.OnSite;
                    //    create.Hours = item.Hours;
                    //    create.Note = item.Note;
                    //}

                    List<WorkReportAttachedFiles> workReportAttachedFiles = new List<WorkReportAttachedFiles>();
                    if (input.AttachedFiles != null)
                    {
                        workReportAttachedFiles.AddRange(input.AttachedFiles.Select(e => new WorkReportAttachedFiles
                        {
                            FileName = e.FileName,
                            FilePath = e.FilePath
                        }));
                    }

                    create.WorkReportAttachedFiles = workReportAttachedFiles;

                    await _workReportRepository.InsertAndGetIdAsync(create);
                }
                else
                {
                    var update = this._workReportRepository.GetAll().Where(e => e.Id == input.Id).FirstOrDefault();
                    if (update == null)
                    {
                        throw new UserFriendlyException("Not found");
                    }

                    update.ProjectId = input.ProjectId;
                    update.SprintId = input.SprintId;
                    update.ModuleId = input.ModuleId;
                    update.DeclarationDate = input.DeclarationDate.ToLocalTime();
                    update.Status = input.Status;
                    update.JobId = input.JobId;
                    update.KindOfJob = input.KindOfJob;
                    update.Type = input.Type;
                    update.OnSite = input.OnSite;
                    update.Hours = input.Hours;
                    update.Note = input.Note;
                    //foreach (var item in input.JobList)
                    //{
                    //    update.JobId = item.JobId;
                    //    update.KindOfJob = item.KindOfJob;
                    //    update.Type = item.Type;
                    //    update.OnSite = item.OnSite;
                    //    update.Hours = item.Hours;
                    //    update.Note = item.Note;
                    //}

                    foreach (var item in input.AttachedFiles)
                    {
                        var file = new WorkReportAttachedFiles
                        {
                            WorkReportId = input.Id,
                            FileName = item.FileName,
                            FilePath = item.FilePath
                        };

                        await _workReportAttachedFilesRepository.InsertAsync(file);
                    }

                    await _workReportRepository.UpdateAsync(update);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DeleteWorkReportAsync(int id)
        {
            if (id == 0 || id == null)
            {
                throw new UserFriendlyException("Input is null");
            }
            await this._workReportRepository.DeleteAsync(id);
        }

        public async Task<List<LookupTableDto>> GetAllSprint(int ProjectId)
        {
            var query = await this._sprintRepository.GetAll().Where(w => w.ProjectId == ProjectId).Select(e => new LookupTableDto
            {
                Id = e.Id,
                DisplayName = e.SprintName
            }).ToListAsync();
            return query;
        }

        public async Task<List<LookupTableDto>> GetAllModule(int ProjectId)
        {
            var query = await this._moduleRepository.GetAll().Where(w => w.ProjectId == ProjectId).Select(e => new LookupTableDto
            {
                Id = e.Id,
                DisplayName = e.ModuleName
            }).ToListAsync();
            return query;
        }

        public async Task<List<LookupTableDto>> GetAllJobBySprintId(int sprintId)
        {
            var query = await this._jobRepository.GetAll().Where(e => e.SprintId == sprintId).Select(e => new LookupTableDto
            {
                Id = e.Id,
                DisplayName = e.JobName
            }).ToListAsync();
            return query;
        }

        public async Task<List<LookupTableDto>> GetAllKindOfJob()
        {
            List<LookupTableDto> result = new List<LookupTableDto>();
            foreach (var item in GlobalModel.KindOfJob)
            {
                result.Add(new LookupTableDto
                {
                    Id = item.Key,
                    DisplayName = item.Value
                });
            }
            return await Task.FromResult(result);
        }

        public async Task<List<LookupTableDto>> GetAllType()
        {
            List<LookupTableDto> result = new List<LookupTableDto>();
            foreach (var item in GlobalModel.Type)
            {
                result.Add(new LookupTableDto
                {
                    Id = item.Key,
                    DisplayName = item.Value
                });
            }
            return await Task.FromResult(result);
        }

        public async Task<FileDto> DownloadFileUpload(string linkFile)
        {
            if (string.IsNullOrEmpty(linkFile))
            {
                throw new UserFriendlyException("Link file is null");
            }

            var fileName = linkFile.Split(Path.DirectorySeparatorChar).Last();
            var path = this._appFolder.WorkReportFileUploadFolder + linkFile.Replace(fileName, string.Empty);

            // _appFolders.DemoFileDownloadFolder : Thư mục chưa file mẫu cần tải
            // _appFolders.TempFileDownloadFolder : Không được sửa
            return await GlobalFunction.DownloadFileMau(fileName, path, this._appFolder.TempFileDownloadFolder);
        }
    }
}
