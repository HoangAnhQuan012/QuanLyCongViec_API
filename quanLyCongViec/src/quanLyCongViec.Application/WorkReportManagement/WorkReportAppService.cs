using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using quanLyCongViec.Authorization.Users;
using quanLyCongViec.Data.Excel.Dtos;
using quanLyCongViec.DbEntities;
using quanLyCongViec.Global;
using quanLyCongViec.Global.Dto;
using quanLyCongViec.Net.MimeTypes;
using quanLyCongViec.WorkReportManagement.Dtos;
using System;
using System.Collections.Generic;
using System.Drawing;
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
        private readonly IRepository<Projects> _projectsRepository;
        public WorkReportAppService(
                IRepository<WorkReport> workReportRepository,
                IRepository<WorkReportAttachedFiles, long> workReportAttachedFilesRepository,
                IAppFolder appFolder,
                IRepository<Sprint> sprint,
                IRepository<Module> moduleRepository,
                IRepository<Job> jobRepository,
                IRepository<ProjectUser, int> projectUserRepository,
                IRepository<User, long> userRepository,
                IRepository<Units> UnitsRepository,
                IRepository<Projects> projectsRepository
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
            _projectsRepository = projectsRepository;
        }

        public async Task<PagedResultDto<GetAllWorkReportForViewDto>> GetAllWorkReport(GetAllInputDtoWorkReport input)
        {
            try
            {
                var userId = this.AbpSession.UserId;

                var getUnitId = await this._userRepository.GetAll().Where(e => e.Id == userId).Select(e => e.UnitId).FirstOrDefaultAsync();
                var isManager = await this._UnitsRepository.GetAll().Where(e => e.Id == getUnitId).Select(e => e.ParentUnitId).FirstOrDefaultAsync();

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
                                 UserName = user.Name,
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

                var entity = await this._workReportRepository.GetAll().Where(w => w.Id == id).FirstOrDefaultAsync();
                WorkReportForViewDto output = new WorkReportForViewDto();

                output.SpintName = this._sprintRepository.GetAll().Where(e => e.Id == entity.SprintId).Select(e => e.SprintName).FirstOrDefault();
                output.SprintId = entity.SprintId;
                output.Id = entity.Id;
                output.ModuleId = entity.ModuleId;
                output.JobId = entity.JobId;
                output.KindOfJobId = entity.KindOfJob;
                output.TypeId = entity.Type;
                output.ModuleName = this._moduleRepository.GetAll().Where(e => e.Id == entity.ModuleId).Select(e => e.ModuleName).FirstOrDefault();
                output.DeclarationDate = entity.DeclarationDate.ToLocalTime();
                output.JobName = this._jobRepository.GetAll().Where(e => e.Id == entity.JobId).Select(e => e.JobName).FirstOrDefault();
                output.KindOfJobName = GlobalModel.KindOfJob[entity.KindOfJob];
                output.TypeName = GlobalModel.Type[entity.Type];
                output.OnSite = entity.OnSite;
                output.Hours = entity.Hours;
                output.Note = entity.Note;
                output.StatusId = entity.Status;
                var listDinhKem = await this._workReportAttachedFilesRepository.GetAll().Where(e => e.WorkReportId == id).ToListAsync();

                if (listDinhKem.Count > 0)
                {
                    foreach (var item in listDinhKem)
                    {
                        workReportAttachedFiles.Add(item);
                    }
                }

                output.ListFile = workReportAttachedFiles;

                return output;
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

        [HttpGet]
        public async Task<FileDto> ExportExcel(GetAllInputDtoWorkReport input)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                if (input == null)
                {
                    throw new UserFriendlyException("Input is null");
                }

                input.SkipCount = 0;
                input.MaxResultCount = int.MaxValue;

                var list = await this.GetAllWorkReport(input);

                using var package = new ExcelPackage();

                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("WorkReport");
                var nameStyle = package.Workbook.Styles.CreateNamedStyle("HyperLink");
                nameStyle.Style.Font.UnderLine = true;
                nameStyle.Style.Font.Color.SetColor(Color.Blue);

                worksheet.Cells[1, 1].Value = "STT";
                worksheet.Cells[1, 2].Value = "Date";
                worksheet.Cells[1, 3].Value = "Sprint";
                worksheet.Cells[1, 4].Value = "Module";
                worksheet.Cells[1, 5].Value = "Job";
                worksheet.Cells[1, 6].Value = "Time";
                worksheet.Cells[1, 7].Value = "Kind of job";
                worksheet.Cells[1, 8].Value = "Type";
                worksheet.Cells[1, 9].Value = "Status";
                worksheet.Cells[1, 10].Value = "Reportee";

                using (var range = worksheet.Cells[1, 1, 1, 10])
                {
                    using var font = new Font("Calibri", 12, FontStyle.Bold);
                    range.Style.Font.Bold = true;
                    range.Style.Font.SetFromFont(font);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                }

                var rowStart = 2;
                var stt = 1;
                list.Items.ToList().ForEach(item =>
                {
                    var getJobName = item.GetReportDetails.Select(e => e.JobName).FirstOrDefault();
                    var getKindOfJobName = item.GetReportDetails.Select(e => e.KindOfJobName).FirstOrDefault();
                    var getTypeName = item.GetReportDetails.Select(e => e.TypeName).FirstOrDefault();

                    worksheet.Cells[rowStart, 1].Value = stt;
                    worksheet.Cells[rowStart, 2].Value = item.DeclarationDate.ToString("dd/MM/yyyy");
                    worksheet.Cells[rowStart, 3].Value = item.SprineName;
                    worksheet.Cells[rowStart, 4].Value = item.ModuleName;
                    worksheet.Cells[rowStart, 5].Value = getJobName;
                    worksheet.Cells[rowStart, 6].Value = item.Hours;
                    worksheet.Cells[rowStart, 7].Value = getKindOfJobName;
                    worksheet.Cells[rowStart, 8].Value = getTypeName;
                    worksheet.Cells[rowStart, 9].Value = item.Status;
                    worksheet.Cells[rowStart, 10].Value = item.UserName;

                    stt++;
                    rowStart++;

                    using (ExcelRange dataRange = worksheet.Cells[1, 1, rowStart - 1, 10])
                    {
                        dataRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        dataRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        dataRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        dataRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }
                });

                worksheet.Cells.AutoFitColumns();
                worksheet.PrinterSettings.FitToHeight = 1;

                var fileName = string.Join(".", new string[] { "WorkReport", "xlsx" });

                using (var stream = new MemoryStream())
                {
                    package.SaveAs(stream);
                };

                var file = new FileDto(fileName, MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet);
                var filePath = Path.Combine(this._appFolder.TempFileDownloadFolder, file.FileToken);
                package.SaveAs(new FileInfo(filePath));
                return file;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> GetProjectName(int projectId)
        {
            var result = await this._projectsRepository.GetAll().Where(e => e.Id == projectId).Select(e => e.ProjectName).FirstOrDefaultAsync();
            return result;
        }
    }
}
