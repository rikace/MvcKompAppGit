using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using System.Collections.Specialized;
using System.Web;
using Newtonsoft.Json;
using System.Configuration;

namespace UploadDemo.Controllers
{
    public class FileUploadController : ApiController
    {

        private string fileFolder(string username)
        {
            /* TODO: Add a filepath setting to your <appSettings> in web.config like so:

               <appSettings>
                  <add key="filepath" value="f:\uploads" />
               </appSettings>
              
               This folder must grant full access to asp.net (ex: AuthenticatedUsers)
            */

            var folder = ConfigurationManager.AppSettings["filepath"] + "\\" + username; 

            if (!System.IO.Directory.Exists(folder))
                System.IO.Directory.CreateDirectory(folder);
            return folder;
        }

        /// <summary>
        /// returns the size of the given file on the server.
        /// </summary>
        /// <returns>If the file is there, the size in bytes, otherwise zero.</returns>
        public string GetFileSize()
        {
            string size = "0";

            // get the filename value from the passed in name/value collection object
            NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var username = nvc["username"];
            var filename = nvc["filename"];
            
            // get the local path of this filename
            //var localFilePath = HttpContext.Current.Server.MapPath("~/" + filename);
            var localFilePath = fileFolder(username) + "\\" + filename;

            // if the file is there, read the length
            if (System.IO.File.Exists(localFilePath))
            {
                var info = new System.IO.FileInfo(localFilePath);
                size = info.Length.ToString();
            }
            return size;
        }

        public void Post()
        {
            // get the data passed in
            var httpRequest = HttpContext.Current.Request;
            var data = httpRequest.BinaryRead(httpRequest.ContentLength);

            // dataString is the serialized object passed from javascript
            var dataString = System.Text.UTF8Encoding.UTF8.GetString(data);
            // turn it into a FileChunk
            FileChunk obj = JsonConvert.DeserializeObject<FileChunk>(dataString);

            // pull out all the variables we need
            string username = obj.username;
            string filename = obj.filename;
            int numchunks = obj.numchunks;
            int chunk = obj.chunk;
            int chunksize = obj.chunksize;
            int position = obj.position;

            if (obj.data != "")
            {

                // deserialize the file chunk data using hex2bin,
                // a simple deserializer that works on the client 
                // and server with no dependencies.
                //byte[] filedata = hex2bin(obj.data);
                byte[] filedata = Convert.FromBase64String(obj.data);


                // write binary data to the file. 

                // you may need to change the path
                //var localFilePath = HttpContext.Current.Server.MapPath("~/" + filename);
                var localFilePath = fileFolder(username) + "\\" + filename;

                // if this is the first chunk, delete the file if it exists.
                if (chunk == 0)
                {
                    if (File.Exists(localFilePath))
                    {
                        File.Delete(localFilePath);
                    }
                }

                // open the file, seek to the position, and write the chunk
                Stream stream = null;
                try
                {
                    stream = File.Open(localFilePath,
                        FileMode.OpenOrCreate,
                        FileAccess.Write, FileShare.Read);

                    stream.Seek(position, SeekOrigin.Begin);
                    if (filedata.Length >= chunksize)
                        stream.Write(filedata, 0, chunksize);
                    else
                        stream.Write(filedata, 0, filedata.Length);

                    stream.Close();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        // simple binary deserializer
        private byte[] hex2bin(string data)
        {
            int l = data.Length / 2;
            var output = new byte[l];
            for (int i = 0; i < data.Length; i += 2)
            {
                byte b = Convert.ToByte(data.Substring(i, 2), 16);
                output[i / 2] = b;
            }
            return output;
        }
    }

    public class FileChunk
    {
        public string username;
        public string filename;
        public int numchunks;
        public int chunk;
        public int position;
        public int chunksize;
        public string data;
    }
}
