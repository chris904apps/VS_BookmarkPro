using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BookmarkPro.Models.Enumerations;

namespace BookmarkPro.Models
{
    public interface IMark
    {
         Guid id { get; set; }
        MarkType Type { get; set; }
         string Name { get; set; }
         string Description { get; set; }

         List<MarkTag> Tags { get; set; }

        void SetLocation(ITextSnapshot section);
    }
}
