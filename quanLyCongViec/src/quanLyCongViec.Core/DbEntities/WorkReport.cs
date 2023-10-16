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
    [Table("WorkReport")]
    public class WorkReport : FullAuditedEntity, IMayHaveTenant
    {
        public virtual int? TenantId { get; set; }
        public virtual int ProjectId { get; set; }
        public virtual int SprintId { get; set; }
        public virtual int ModuleId { get; set; }
        public virtual DateTime DeclarationDate { get; set; }
        public virtual int JobId { get; set; }
        public virtual int KindOfJob { get; set; }
        public virtual int Type { get; set; }
        public virtual bool OnSite { get; set; }
        public virtual string Note { get; set; }
        public virtual double Hours { get; set; }
        public virtual int Status { get; set; }
        public virtual ICollection<WorkReportAttachedFiles> WorkReportAttachedFiles { get; set; }
    }
}
