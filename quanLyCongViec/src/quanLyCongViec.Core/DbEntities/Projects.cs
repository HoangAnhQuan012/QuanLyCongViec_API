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
    [Table("Projects")]
    public class Projects : FullAuditedEntity, IMayHaveTenant
    {
        public virtual int? TenantId { get; set; }
        public virtual string ProjectName { get; set; }
        public virtual string Customer { get; set; }
        public virtual long ProjectManagerId { get; set; }
        public virtual string ProjectManagerName { get; set; }
        public virtual int Status { get; set; }
        public virtual DateTime StartDate { get; set; }
        public virtual DateTime EndDate { get; set; }
        public virtual string Note { get; set; }
        public virtual int UserId { get; set; }
        public virtual ICollection<ProjectUser> ProjectUsers { get; set; }
        public virtual ICollection<ProjectAttachedFiles> ProjectAttachedFiles { get; set; }
    }
}
