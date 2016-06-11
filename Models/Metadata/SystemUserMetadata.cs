using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace twMVCWorkshop.Models
{
    [MetadataType(typeof(SystemUserMetadata))]
    public partial class SystemUser
    {
        private class SystemUserMetadata
        {
            public System.Guid ID { get; set; }
            [DisplayName("姓名")]
            [StringLength(20)]
            public string Name { get; set; }
            [DisplayName("帳號")]
            [StringLength(20)]
            public string Account { get; set; }
            [DisplayName("密碼")]
            public string Password { get; set; }
            public string Salt { get; set; }
            [DisplayName("電子郵件")]
            [EmailAddress]
            public string Email { get; set; }
            [DisplayName("建檔人")]
            [UIHint("SystemUserName")]
            public System.Guid CreateUser { get; set; }
            [DisplayName("建檔日期")]
            public System.DateTime CreateDate { get; set; }
            [DisplayName("修改人")]
            [UIHint("SystemUserName")]
            public Nullable<System.Guid> UpdateUser { get; set; }
            [DisplayName("修改日期")]
            public System.DateTime UpdateDate { get; set; }
        }
    }
}