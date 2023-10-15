using Abp.Domain.Repositories;
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

        public async Task<List<LookupTableDto>> GetAllSprint()
        {
            var query = await this._sprintRepository.GetAll().Select(e => new LookupTableDto
            {
                Id = e.Id,
                DisplayName = e.SprintName
            }).ToListAsync();
            return query;
        }

        public async Task<List<LookupTableDto>> GetAllModule()
        {
            var query = await this._moduleRepository.GetAll().Select(e => new LookupTableDto
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
