using BookmarkPro.Data;
using BookmarkPro.Models;
using EnvDTE;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;
using System.IO;

namespace BookmarkPro.Testing
{
    [TestClass]
    public class T_Bookmarks
    {
         static readonly string testFilePath = Path.Combine(Directory.GetCurrentDirectory(), "TestJson.json");

        static RepositoryManager RepositManager { get; set; }
        //class initialize
        //create file
        [ClassInitialize]
        public static void InitializeTestClass(TestContext tstContext)
        {
            RepositManager = new RepositoryManager( testFilePath);
            DeleteTestJsonFile();
            //create test file
            RepositManager.CreateFile();
        }


        //class cleanup
        //delete file
        [ClassCleanup]
        public static void CleanupTestClass()
        {
            DeleteTestJsonFile();
        }
        
        /// <summary>
        /// Clear test json file
        /// </summary>
        [TestInitialize]
        public  void Setup()
        {
            //check if test file already exist
            if (RepositManager.FileExist())
            {
                //delete test file
                RepositManager.ClearFile();
            }
        }

        #region Tests
        [TestMethod]
        public void AddBookmark()
        {
            IMark nBookmark = CreateTestBookmark("Bookmark one", "First bookmark");



            //save to file
            RepositManager.Bookmarks.Add((Bookmark)nBookmark);
            RepositManager.Save();

            IMark results = RepositManager.Bookmarks.Find((Bookmark)nBookmark);

            Assert.IsNotNull(results);
            Assert.IsNotNull(results.id);
        }

        [TestMethod]
        public void Findbookmark()
        {

            //save to file
            RepositManager.Bookmarks.Add((Bookmark)CreateTestBookmark("Bookmark one", "First bookmark"));
            RepositManager.Bookmarks.Add((Bookmark)CreateTestBookmark("Bookmark one", "First bookmark"));
            RepositManager.Bookmarks.Add((Bookmark)CreateTestBookmark("Bookmark one", "First bookmark"));
            RepositManager.Bookmarks.Add((Bookmark)CreateTestBookmark("Bookmark one", "First bookmark"));
            RepositManager.Bookmarks.Add((Bookmark)CreateTestBookmark("Bookmark one", "First bookmark"));
            RepositManager.Save();

            var results = RepositManager.Bookmarks.FindAll();
            var count = results.Count();

            Assert.IsNotNull(results);
            Assert.IsTrue(count > 5);
        }

        [TestMethod]
        public void RemoveBookmark()
        {
            //create bookmark for deleting
            IMark nBookmark = CreateTestBookmark("Bookmark one", "First bookmark");

            //save bookmark
            //save to file
            RepositManager.Bookmarks.Add((Bookmark)nBookmark);
            RepositManager.Save();

            //get id 
            var id = nBookmark.id;
            //delete bookmark
            RepositManager.Bookmarks.Delete(id);
            RepositManager.Save();

            IMark results = RepositManager.Bookmarks.FindById(id);

            Assert.IsNull(results);
        }

        [TestMethod]
        public void ModifyBookmark()
        {
            //create bookmark for deleting
            IMark nBookmark = CreateTestBookmark("Bookmark Unmodified", "Original description");

            //save bookmark
            RepositManager.Bookmarks.Add((Bookmark)nBookmark);
            RepositManager.Save();

            //get id 
            var id = nBookmark.id;
            //retrieve bookmark
            IMark toModify = RepositManager.Bookmarks.FindById(id);
            //modify
            string modifedName = "Modified Bookmark";
            string modifiedDescription = "New description";

            toModify.Name = modifedName;
            toModify.Description = modifiedDescription;

            //save
            RepositManager.Bookmarks.Update((Bookmark)toModify);
            RepositManager.Save();

            //retrieve
            IMark modifedBookmark = RepositManager.Bookmarks.FindById(id);
            //assert
            Assert.AreEqual<string>(modifedName, modifedBookmark.Name);
            Assert.AreEqual<string>(modifiedDescription, modifedBookmark.Description);
        }

        [TestMethod]
        public void MoveBookmark()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void SortBookmarks()
        {
            //add bookmarks
            IList<Bookmark> unordered = new List<Bookmark>();
            unordered.Add(CreateTestBookmark("z name", "First in"));
            unordered.Add(CreateTestBookmark("Y name", "Second in"));
            unordered.Add(CreateTestBookmark("r name", "Third in"));
            unordered.Add(CreateTestBookmark("f name", "Fourth in"));
            unordered.Add(CreateTestBookmark("a name", "Fifth in"));

            //save bookmarks
            RepositManager.Bookmarks.Add(unordered);
            RepositManager.Save();

            var sortedList = RepositManager.Bookmarks.FindAll().OrderBy(x => x.Name).ToList();

            Assert.IsTrue(sortedList.First().Name.StartsWith("a"));
            Assert.IsTrue(sortedList.Last().Name.StartsWith("z"));
        }

        [TestMethod]
        public void FilterBookmarks()
        {
            //add bookmarks
            IList<Bookmark> unordered = new List<Bookmark>();
            unordered.Add(CreateTestBookmark("Filter bookmark", "First in", "Clean up"));
            unordered.Add(CreateTestBookmark("Filter bookmark two", "First in", "Clean up"));

            unordered.Add(CreateTestBookmark("another filter bookmark", "Second in", null, "ISSUE"));
            unordered.Add(CreateTestBookmark("another filter bookmark", "Second in", null, "ISSUE"));

            RepositManager.Bookmarks.Add(unordered);
            RepositManager.Save();

            IMark filter = new Bookmark();
            filter.Tags = new List<MarkTag>();
            filter.Tags.Add(new MarkTag() { Catagory = Enumerations.TagCatagories.TFS.ToString() });

            var filter1 = RepositManager.Bookmarks.FindWhere((Bookmark)filter);

            var filter2 = RepositManager.Bookmarks.FindAll()
                .Where(x => x.Tags.Any(a => a.Catagory == "TFS" && a.Tag.Catagory == "ISSUE"))
                .ToList();
            //call bookmark manager
            //retrieve bookmarks for solution
            Assert.IsFalse(!filter1.Any(a => a.Tags.Any(a1 => a1.Tag.Catagory == Enumerations.TagCatagories.TODO.ToString())));
            Assert.IsFalse(!filter2.Any(a => a.Tags.Any(a1 => a1.Tag.Catagory == Enumerations.TagCatagories.TFS.ToString())));

        }

        #endregion

        #region Private Methods
        private  Bookmark CreateTestBookmark(string name, string description, string TodoTag = null, string TfsTag = null)
        {
            Bookmark nBookmark = new Bookmark();
            //create a bookmark object
            nBookmark.Type = Enumerations.MarkType.BookMark;
            nBookmark.Name = name;
            nBookmark.Description = description;
            //set location
            ITextSnapshot section = new TestTextSnapshot();
            nBookmark.SetLocation(section);
            //set tags
            if (TodoTag != null)
            {
                MarkTag tag1 = new MarkTag();
                tag1.Catagory = Enumerations.TagCatagories.TODO.ToString();
                tag1.Tag = new MarkTag() { Catagory = TodoTag };
                nBookmark.Tags.Add(tag1);
            }
            if (TfsTag != null)
            {
                MarkTag tag1 = new MarkTag();
                tag1.Catagory = Enumerations.TagCatagories.TFS.ToString();
                tag1.Tag = new MarkTag() { Catagory = TfsTag };
                nBookmark.Tags.Add(tag1);
            }
            return nBookmark;
        }

        private static void DeleteTestJsonFile()
        {
            //check if test file already exist
            if (RepositManager.FileExist())
            {
                //delete test file
                RepositManager.DeleteFile();
            }
        }
        #endregion

    }

    internal class TestTextSnapshot : ITextSnapshot
    {
        public char this[int position]
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IContentType ContentType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int Length
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int LineCount
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IEnumerable<ITextSnapshotLine> Lines
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ITextBuffer TextBuffer
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ITextVersion Version
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count)
        {
            throw new NotImplementedException();
        }

        public ITrackingPoint CreateTrackingPoint(int position, PointTrackingMode trackingMode)
        {
            throw new NotImplementedException();
        }

        public ITrackingPoint CreateTrackingPoint(int position, PointTrackingMode trackingMode, TrackingFidelityMode trackingFidelity)
        {
            throw new NotImplementedException();
        }

        public ITrackingSpan CreateTrackingSpan(Span span, SpanTrackingMode trackingMode)
        {
            throw new NotImplementedException();
        }

        public ITrackingSpan CreateTrackingSpan(int start, int length, SpanTrackingMode trackingMode)
        {
            throw new NotImplementedException();
        }

        public ITrackingSpan CreateTrackingSpan(Span span, SpanTrackingMode trackingMode, TrackingFidelityMode trackingFidelity)
        {
            throw new NotImplementedException();
        }

        public ITrackingSpan CreateTrackingSpan(int start, int length, SpanTrackingMode trackingMode, TrackingFidelityMode trackingFidelity)
        {
            throw new NotImplementedException();
        }

        public ITextSnapshotLine GetLineFromLineNumber(int lineNumber)
        {
            throw new NotImplementedException();
        }

        public ITextSnapshotLine GetLineFromPosition(int position)
        {
            throw new NotImplementedException();
        }

        public int GetLineNumberFromPosition(int position)
        {
            throw new NotImplementedException();
        }

        public string GetText()
        {
            throw new NotImplementedException();
        }

        public string GetText(Span span)
        {
            throw new NotImplementedException();
        }

        public string GetText(int startIndex, int length)
        {
            throw new NotImplementedException();
        }

        public char[] ToCharArray(int startIndex, int length)
        {
            throw new NotImplementedException();
        }

        public void Write(TextWriter writer)
        {
            throw new NotImplementedException();
        }

        public void Write(TextWriter writer, Span span)
        {
            throw new NotImplementedException();
        }
    }

}
