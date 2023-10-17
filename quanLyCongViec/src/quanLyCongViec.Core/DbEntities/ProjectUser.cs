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
    [Table("ProjectUser")]
    public class ProjectUser : FullAuditedEntity<int>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public int ProjectsId { get; set; }
        public int UserId { get; set; }
    }
}
