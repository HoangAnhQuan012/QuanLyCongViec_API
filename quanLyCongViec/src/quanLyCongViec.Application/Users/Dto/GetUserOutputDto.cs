using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using quanLyCongViec.Authorization.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quanLyCongViec.Users.Dto
{
    [AutoMap(typeof(User))]
    public class GetUserOutputDto : EntityDto<int>
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string UserName { get; set;}
        public int? UnitId { get; set; }
        public string EmailAddress { get; set; }
        public bool IsActive { get; set; }
    }
}
