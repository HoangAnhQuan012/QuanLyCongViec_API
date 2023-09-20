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
        public int? TenantId { get; set; }
        public string ProjectName { get; set; }
        public string Customer { get; set; }
        public long ProjectManagerId { get; set; }
        public string ProjectManagerName { get; set; }
        public int? Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Note { get; set; }
        public List<ProjectAttachedFiles> ProjectAttachedFiles { get; set; }
    }
}
