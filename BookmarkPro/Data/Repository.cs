using System;
using System.Collections.Generic;
using BookmarkPro.Models;
using Newtonsoft.Json;
using System.IO;
using System.Linq;

namespace BookmarkPro.Data
{
    public class Repository<T> where T: IMark //where R : RepositoryManager
    {
        #region Properties
        static public RepositoryManager _Repositiory;
        private JsonContext JContext;


        public List<T> CreatedMarks { get; set; }
        public List<Guid> DeleteMarks { get; set; }

        public SolutionMarks SolutionBookmarks { get; private set; }
        #endregion

        #region Constructor(s)
        public Repository(JsonContext jsonContext)
        {
            JContext = jsonContext;
        }
        #endregion

        #region Retrieval Methods
        internal List<T> FindAll()
        {
            //_Repositiory
            JsonConverter c;

            //convert from JSON to list
            //return list
            using (var sr = new StreamReader(File.Open(JContext.FullFilePath, FileMode.OpenOrCreate)))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                return JContext.SerializerDeserializer.Deserialize<SolutionMarks>(jsonTextReader).Bookmarks as List<T>;
            }

        }

        internal IMark Find(T nBookmark)
        {
            //TODO: can i do lazy load, or filter inside of stream?
            IQueryable<T> l = Finder(nBookmark);
            return l.First();
        }

        private IQueryable<T> Finder(T nBookmark)
        {
            var l = FindAll().AsQueryable();

            l = l.Where<T>(x => x.Type == nBookmark.Type);

            if (nBookmark.id != null)
                l = l.Where<T>(x => x.id == nBookmark.id);
            if (!String.IsNullOrWhiteSpace(nBookmark.Name))
                l = l.Where<T>(x => x.Name.ToUpper().Trim().Contains(nBookmark.Name.ToUpper().Trim()));
            if (!String.IsNullOrWhiteSpace(nBookmark.Description))
                l = l.Where<T>(x => x.Description.ToUpper().Trim().Contains(nBookmark.Description.ToUpper().Trim()));
            if (nBookmark.Tags != null)
            {
                foreach (var tag in nBookmark.Tags)
                {
                    l = l.Where<T>(x => x.Tags.Contains(tag));
                }
            }

            return l;
        }

        internal List<T> FindWhere(T nBookmark)
        {
            IQueryable<T> l = Finder(nBookmark);
            return l.ToList();
        }

        internal T FindById(Guid id)
        {
            return FindAll().FirstOrDefault(x => x.id == id);
        }

        #endregion


        #region Data Management Methods
        internal void Update(T toModify)
        {
            if (CreatedMarks == null)
                CreatedMarks = new List<T>();

            //throw new NotImplementedException();
            if (toModify.id != null && CreatedMarks.Any(x=> x.id   == toModify.id))
                {
                //replace the one that is currently in the list with new modification
                for (int i = 0; i < CreatedMarks.Count; i++)
                {
                    if(CreatedMarks[i].id == toModify.id)
                    { CreatedMarks[i] = toModify; }
                }
            }
            else
            {
                //add to list
                CreatedMarks.Add(toModify);
            }
        }

        internal void Add(IList<T> unordered)
        {
            //throw new NotImplementedException();
            if (CreatedMarks == null)
                CreatedMarks = new List<T>();
            CreatedMarks.AddRange(unordered);
        }

        internal void Add(T nBookmark)
        {
            //throw new NotImplementedException();
            //
            //if(CreatedMarks.Any(x=> x.id == nBookmark.id) && nBookmark.id != null))
            //    throw new ApplicationException("Can not add as Bookmark areadl")
            if (CreatedMarks == null)
                CreatedMarks = new List<T>();
            CreatedMarks.Add(nBookmark);
        }

        internal void Delete(Guid id)
        {
            //throw new NotImplementedException();
            if (DeleteMarks == null)
                DeleteMarks = new List<Guid>();
            DeleteMarks.Add(id);
        }

        #endregion
    }
}