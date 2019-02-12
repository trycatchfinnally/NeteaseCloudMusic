using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeteaseCloudMusic.Core.Domain.Localization
{
   public  class Language
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LanguageCulture { get; set; }
        public int DisplayOrder { get; set; }
        public bool Published { get; set; }

    }
}
