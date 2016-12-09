using System.Collections.Generic;
using static BookmarkPro.Models.Enumerations;

namespace BookmarkPro.Models
{
    public interface IMarkTag
    {
        //TagCatagories Catagory { get; set; }
        string Catagory { get; set; }
        IMarkTag Tag { get; set; }
    }
}