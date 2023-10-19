using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using quanLyCongViec.Authorization.Users;
using quanLyCongViec.DbEntities;
using quanLyCongViec.Reports.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static quanLyCongViec.Global.GlobalConst;

namespace quanLyCongViec.Reports
{
    public class ReportsAppService : quanLyCongViecAppServiceBase
    {
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<WorkReport> _workReportRepository;
        public ReportsAppService(IRepository<User, long> userRepository, IRepository<WorkReport> workReportRepository)
        {
            _userRepository = userRepository;
            _workReportRepository = workReportRepository;
        }

        public async Task<GetAllForViewDto> GetAllDataReports()
        {
            var listUser = await this._userRepository.GetAll().Select(e => e.UserName).ToListAsync();

            var listUserId = await this._userRepository.GetAll().Select(e => e.Id).ToListAsync();

            // Lấy ra số giờ logeed work của từng User
            var listHoursWorked = new List<double>();
            var listDaysLogged = new List<int>();
            foreach (var id in listUserId)
            {
                var listWorkReport = await this._workReportRepository.GetAll().Where(e => e.CreatorUserId == id && e.Status == (int)WorkReportStatus.Approved).ToListAsync();
                var hoursWorked = listWorkReport.Sum(e => e.Hours);
                listHoursWorked.Add(hoursWorked);

                // Lấy ra số lần log work của từng người dùng đã được duyệt
                var listWorkReportApproved = await this._workReportRepository.GetAll().Where(e => e.CreatorUserId == id && e.Status == (int)WorkReportStatus.Approved).ToListAsync();
                var daysLogged = listWorkReportApproved.Count();
                listDaysLogged.Add(daysLogged);
            }

            return new GetAllForViewDto
            {
                ListUserName = listUser,
                HoursWorked = listHoursWorked,
                DaysLogged = listDaysLogged
            };
        }
    }
}
