using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace twMVCWorkshop.Models
{
    [MetadataType(typeof(CategoryMetadata))]
    public partial class Category
    {
        private class CategoryMetadata
        {
            [UIHint("SystemUserName")]
            public Guid CreateUser { get; set; }
            [UIHint("SystemUserName")]
            public Nullable<Guid> UpdateUser { get; set; }
        }
    }
}