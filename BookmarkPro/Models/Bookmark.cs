using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookmarkPro.Models
{
    public class Bookmark : IMark
    {
        public Bookmark()
        {
            Tags = new List<MarkTag>();
        }
        public string Description { get; set; }

        public Guid id { get; set; }

        public string Name { get; set; }

        //public List<IMarkTag> Tags { get; set; }

        public Enumerations.MarkType Type { get; set; }

        public List<MarkTag> Tags { get; set; }

        public void SetLocation(ITextSnapshot section)
        {
            //TODO:
        }
    }
}
