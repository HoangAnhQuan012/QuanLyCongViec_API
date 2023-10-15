using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quanLyCongViec.DbEntities
{
    [Table("Job")]
    public class Job : FullAuditedEntity, IMayHaveTenant
    {
        public virtual int? TenantId { get; set; }
        public virtual string JobName { get; set; }
        public virtual int SprintId { get; set; }
        public virtual int ProjectManagerId { get; set; }
    }
}
