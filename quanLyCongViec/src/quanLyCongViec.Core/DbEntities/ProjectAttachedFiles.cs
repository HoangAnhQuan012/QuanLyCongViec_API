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
    [Table("ProjectAttachedFiles")]
    public class ProjectAttachedFiles : FullAuditedEntity<long>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public long ProjectId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}
