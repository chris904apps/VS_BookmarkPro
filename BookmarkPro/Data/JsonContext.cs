using Newtonsoft.Json;
using System;
using System.IO;

namespace BookmarkPro.Data
{
    public  class JsonContext
    {
        public string FullFilePath { get; set; }

        public JsonConverter _JsonConverter { get; set; }

        public JsonSerializer SerializerDeserializer {
        get
            {
                return new JsonSerializer();
            }
        }

        //public static FileStream fileStream
        //{
        //    get { return File.Open(FullFilePath, FileMode.OpenOrCreate); }
        //}

        public JsonContext (string path)
        {
            FullFilePath = path;
        }

        public bool FileExist()
        {
            //check if path property is set
            if (String.IsNullOrWhiteSpace(FullFilePath))
            {
                throw new ApplicationException("JsonContext 'FullFilePath' is not set in repository");
            }

            return File.Exists(FullFilePath);
        }
    }
}