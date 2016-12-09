using System;

namespace BookmarkPro.Models
{
    public   class MarkTag : IMarkTag
    {
        public string Catagory { get; set; }

        public IMarkTag Tag { get; set; }

    }
}