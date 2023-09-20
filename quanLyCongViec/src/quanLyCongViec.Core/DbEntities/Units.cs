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
    [Table("Units")]
    public class Units : FullAuditedEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public string UnitName { get; set; }
        public int? ParentUnitId { get; set; }
        public string Note { get; set; }
    }
}
