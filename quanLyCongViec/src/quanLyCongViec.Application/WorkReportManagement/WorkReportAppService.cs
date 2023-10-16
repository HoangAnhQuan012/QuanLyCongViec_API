using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
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
        public WorkReportAppService(
                IRepository<WorkReport> workReportRepository,
                IRepository<WorkReportAttachedFiles, long> workReportAttachedFilesRepository,
                IAppFolder appFolder,
                IRepository<Sprint> sprint,
                IRepository<Module> moduleRepository,
                IRepository<Job> jobRepository
            )
        {
            _workReportRepository = workReportRepository;
            _workReportAttachedFilesRepository = workReportAttachedFilesRepository;
            _appFolder = appFolder;
            _sprintRepository = sprint;
            _moduleRepository = moduleRepository;
            _jobRepository = jobRepository;
        }

        public async Task<PagedResultDto<GetAllWorkReportForViewDto>> GetAllWorkReport(PagedResultRequestDto input)
        {
            try
            {
                var result = from report in this._workReportRepository.GetAll()
                             from sprint in this._sprintRepository.GetAll().Where(e => e.Id == report.SprintId)
                             from module in this._moduleRepository.GetAll().Where(e => e.Id == report.ModuleId)
                             select new GetAllWorkReportForViewDto()
                             {
                                 Id = report.Id,
                                 DeclarationDate = report.DeclarationDate,
                                 SprineName = sprint.SprintName,
                                 ModuleName = module.ModuleName,
                                 Hours = report.Hours,
                                 Status = GlobalModel.WorkReportStatus[report.Status],
                                 StatusId = report.Status,
                                 GetReportDetails = (from job in this._jobRepository.GetAll().Where(e => e.Id == report.JobId)
                                                     from work in this._workReportRepository.GetAll().Where(e => e.Id == report.Id)
                                                     from dinhKem in this._workReportAttachedFilesRepository.GetAll().Where(e => e.WorkReportId == report.Id)
                                                     select new GetAllDetails
                                                     {
                                                         JobName = job.JobName,
                                                         KindOfJobName = GlobalModel.KindOfJob[work.KindOfJob],
                                                         TypeName = GlobalModel.Type[work.Type],
                                                         Hours = work.Hours,
                                                     }).ToList()
                             };

                int totalCount = await result.CountAsync();
                var output = await result.PageBy(input).ToListAsync();

                return new PagedResultDto<GetAllWorkReportForViewDto>(totalCount, output);
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
                if (input.Status)
                {
                    update.Status = (int)WorkReportStatus.Approved;
                }
                else if (!input.Status)
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
                        Status = input.Status
                    };

                    foreach (var item in input.JobList)
                    {
                        create.JobId = item.JobId;
                        create.KindOfJob = item.KindOfJob;
                        create.Type = item.Type;
                        create.OnSite = item.OnSite;
                        create.Hours = item.Hours;
                        create.Note = item.Note;
                    }

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
                    foreach (var item in input.JobList)
                    {
                        update.JobId = item.JobId;
                        update.KindOfJob = item.KindOfJob;
                        update.Type = item.Type;
                        update.OnSite = item.OnSite;
                        update.Hours = item.Hours;
                        update.Note = item.Note;
                    }

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
            var path = this._appFolder.ProjectFileUploadFolder + linkFile.Replace(fileName, string.Empty);

            // _appFolders.DemoFileDownloadFolder : Thư mục chưa file mẫu cần tải
            // _appFolders.TempFileDownloadFolder : Không được sửa
            return await GlobalFunction.DownloadFileMau(fileName, path, this._appFolder.TempFileDownloadFolder);
        }
    }
}
