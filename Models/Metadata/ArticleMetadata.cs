using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace twMVCWorkshop.Models
{
    public class ArticleMetadata
    {
        [MetadataType(typeof(ArticleMetadata))]
        public partial class Article
        {
            private class ArticleMetadata
            {
                [UIHint("SystemUserName")]
                public Guid CreateUser { get; set; }
                [UIHint("SystemUserName")]
                public Nullable<Guid> UpdateUser { get; set; }
            }
        }
    }
}