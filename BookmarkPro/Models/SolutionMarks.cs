using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookmarkPro.Models
{
    public class SolutionMarks
    {
        public List<Bookmark> Bookmarks { get; set; }

        public SolutionMarks()
        {
            Bookmarks = new List<Bookmark>();
        }
    }
}
