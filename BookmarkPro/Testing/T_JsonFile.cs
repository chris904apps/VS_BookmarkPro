using BookmarkPro.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookmarkPro.Testing
{
    [TestClass]
    public class T_JsonFile
    {
        static readonly string testFilePath = Path.Combine(Directory.GetCurrentDirectory(),"TestJson.json");

        public static RepositoryManager RepositManager { get; set; }
        [ClassInitialize()]
        public static void Setup( TestContext testCtext)
        {
            //set path where JSON file should be stored
            RepositManager = new RepositoryManager(testFilePath);

        }

        #region Tests
        [TestMethod]
        public void CreateJsonFile()
        {
            ////verify file doesn't exist
            //Assert.IsFalse(RepositManager.FileExist());
            
            //create JSON file
            RepositManager.CreateFile();
            //verify file was created
            Assert.IsTrue(RepositManager.FileExist());

        }

        [TestMethod]
        public void DeleteJsonFile()
        {
            //Verify file exist
            if (!RepositManager.FileExist())
            {
                //create JSON file
                RepositManager.CreateFile();
            }

            //delete JSON file
            RepositManager.DeleteFile();

            //verify file was deleted
            Assert.IsFalse(RepositManager.FileExist());
        }
        #endregion

        [ClassCleanup()]
        public static void CleanUp()
        {
            //Delete file
            if (RepositManager.FileExist())
                RepositManager.DeleteFile();
        }
    }
}
