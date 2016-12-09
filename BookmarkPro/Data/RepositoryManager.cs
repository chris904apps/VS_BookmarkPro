using BookmarkPro.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookmarkPro.Data
{
    //TODO: unit of work pattern
    public class RepositoryManager
    {
        public  JsonContext jsonContext { get; set; }

        public  Repository<Bookmark> Bookmarks;

        public RepositoryManager(string filepath)
        {
            FilePath = filepath;
            jsonContext = new JsonContext(FilePath);

            Bookmarks = new Repository<Bookmark>(jsonContext);

        }
        //internal  RepositoryTable<Group>  Groups;

        public  string FilePath { get; set;}


        internal  void CreateFile()
        {
            try
            {
                using (var filestrm = File.Create(FilePath))
                {
                    //Byte[] info = new UTF8Encoding(true).GetBytes("This is some text in the file.");
                    //// Add some information to the file.

                    //filestrm.Write(info, 0, info.Length);

                }
            }
            catch (DirectoryNotFoundException)
            {
                //TODO: 
            }
            catch (PathTooLongException)
            {
                //TODO: 
            }

        }

        internal  void DeleteFile()
        {
            try
            {
                var empty = new Byte[0];
                File.Delete(FilePath);
            }
            catch (DirectoryNotFoundException)
            {
                //TODO: 
            }
            catch (PathTooLongException)
            {
                //TODO: 
            }
        }

        internal  void Save()
        {
            //http://www.newtonsoft.com/json/help/html/DeserializeWithJsonSerializerFromFile.htm
            //http://www.newtonsoft.com/json/help/html/SerializeWithJsonSerializerToFile.htm

            SolutionMarks solutionMarks = new SolutionMarks();
            //TODO: Can this be done with stream reader? can i read through file removing, adding, and modifying content within the stream?
            using (var sr = new StreamReader(File.Open(jsonContext.FullFilePath, FileMode.OpenOrCreate)))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                //return JContext.SerializerDeserializer.Deserialize<SolutionMarks>(jsonTextReader).Bookmarks;
                //get solution marks
                solutionMarks = jsonContext.SerializerDeserializer.Deserialize<SolutionMarks>(jsonTextReader);

                //check if solutionmarks is null
                if (solutionMarks == null)
                    solutionMarks = new SolutionMarks();

                //delete bookmarks
                if (Bookmarks.DeleteMarks != null)
                {
                    foreach (Guid id in Bookmarks.DeleteMarks)
                    {
                        for (int i = 0; i < solutionMarks.Bookmarks.Count; i++)
                        {
                            if (Bookmarks.DeleteMarks.Contains(solutionMarks.Bookmarks[i].id))
                            {
                                solutionMarks.Bookmarks.Remove(solutionMarks.Bookmarks[i]);
                                break;
                            }
                        }
                    }
                }

                if (Bookmarks.CreatedMarks != null)
                {
                    foreach (var bMark in Bookmarks.CreatedMarks)
                    {
                        //add bookmarks
                        if (solutionMarks.Bookmarks.Any(a => a.id == bMark.id))
                        {
                            //modify bookmarks
                            var index = solutionMarks.Bookmarks.FindIndex(x => x.id == bMark.id);
                            solutionMarks.Bookmarks[index] = bMark;
                        }
                        else
                        {
                            AddBookmark(solutionMarks, bMark);
                        }

                    }
                }

            }
                //save file
                SaveToFile(solutionMarks);
        }

        private  void SaveToFile(SolutionMarks solutionMarks)
        {
            using (StreamWriter sw = new StreamWriter(File.Open(jsonContext.FullFilePath, FileMode.OpenOrCreate)))
            {
                sw.Flush();
                using (JsonWriter jw = new JsonTextWriter(sw))
                {
                    jsonContext.SerializerDeserializer.Serialize(jw, solutionMarks);
                }
                sw.Close();
            }
        }

        private  void AddBookmark(SolutionMarks solutionMarks, Bookmark bMark)
        {
            //create new guid
            Guid nwGuid = Guid.NewGuid();

            //insure new guid not in list of current
            while (solutionMarks.Bookmarks.Any(a=> a.id == nwGuid))
            {
                nwGuid = Guid.NewGuid();
            }
            bMark.id = nwGuid;

            solutionMarks.Bookmarks.Add(bMark);
        }


        internal  void ClearFile()
        {
            try
            {
                var empty = new Byte[0];
                File.WriteAllBytes(FilePath, empty);
            }
            catch (DirectoryNotFoundException)
            {
                //TODO: 
            }
            catch (PathTooLongException)
            {
                //TODO: 
            }
        }

        internal  bool FileExist()
        {
            return jsonContext.FileExist(); 
        }

    }
}
