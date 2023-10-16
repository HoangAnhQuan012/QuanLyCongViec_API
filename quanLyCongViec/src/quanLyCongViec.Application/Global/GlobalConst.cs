using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quanLyCongViec.Global
{
    public class GlobalConst
    {
        public enum ProjectStatus
        {
            NotStarted = 0,
            InProgress,
            Completed
        }

        public enum WorkReportStatus
        {
            WaitForAppoval = 0,
            Approved,
            Rejected
        }
    }
}
