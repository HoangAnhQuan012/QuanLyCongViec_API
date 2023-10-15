using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quanLyCongViec
{
    public class AppFolder : IAppFolder, ISingletonDependency
    {
        public string TempFileDownloadFolder { get; set; }
        public string ProjectFileDownloadFolder { get; set; }
        public string ProjectFileUploadFolder { get; set; }
        public string WorkReportFileDownloadFolder { get; set; }
        public string WorkReportFileUploadFolder { get; set; }
    }
}
