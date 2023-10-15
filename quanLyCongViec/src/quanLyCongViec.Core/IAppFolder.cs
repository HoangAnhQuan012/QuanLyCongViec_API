using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quanLyCongViec
{
    public interface IAppFolder
    {
        string TempFileDownloadFolder { get; }
        string ProjectFileDownloadFolder { get; }
        string ProjectFileUploadFolder { get; }
        string WorkReportFileDownloadFolder { get; }
        string WorkReportFileUploadFolder { get; }
    }
}
