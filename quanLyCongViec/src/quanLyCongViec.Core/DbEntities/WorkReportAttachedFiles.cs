using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quanLyCongViec.DbEntities
{
    [Table("WorkReportAttachedFiles")]
    public class WorkReportAttachedFiles : FullAuditedEntity<long>, IMayHaveTenant
    {
        public virtual int? TenantId { get; set; }
        public virtual int WorkReportId { get; set; }
        public virtual string FileName { get; set; }
        public virtual string FilePath { get; set; }
    }
}
