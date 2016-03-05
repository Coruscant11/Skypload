using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Skypload
{
    class Uploader
    {
        private const string URL = "https://mixtape.moe/upload.php"; // URL d'upload
        private const string FILE_PARAMETER = "files[]"; // Paramètre du fichier

        public Uploader()
        {
            
        }

        /* Upload le fichier, convertis la réponse en JSON et la traduis afin de récupérer l'url */
        public string Upload(string file)
        {
            var s = PostFiles(file);

            JObject json = JObject.Parse(s);
            JArray a = JArray.Parse(json.SelectToken("files").ToString());

            return a[0].SelectToken("url").ToString();
        }

        /* Upload HTTP de fichier */
        private string PostFiles(string files)
        {
            try {

            string url = URL;
 
            string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "multipart/form-data; boundary=" + boundary;
            httpWebRequest.Method = "POST";
            httpWebRequest.KeepAlive = true;
            httpWebRequest.Credentials = System.Net.CredentialCache.DefaultCredentials;

            Stream memStream = new System.IO.MemoryStream();
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
            string formdataTemplate = "\r\n--" + boundary + "\r\nContent-Disposition:  form-data; name=\"{0}\";\r\n\r\n{1}";
            string headerTemplate = "Content-Disposition: form-data; name=\"" + FILE_PARAMETER + "\"; filename=\"{1}\"\r\n Content-Type: application/octet-stream\r\n\r\n";
            memStream.Write(boundarybytes, 0, boundarybytes.Length);

                string header = string.Format(headerTemplate, "files", files);
                byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);

                memStream.Write(headerbytes, 0, headerbytes.Length);
                FileStream fileStream = new FileStream(files, FileMode.Open, FileAccess.Read);

                byte[] buffer = new byte[1024];
                int bytesRead = 0;

                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    memStream.Write(buffer, 0, bytesRead);
                }

                memStream.Write(boundarybytes, 0, boundarybytes.Length);
                    memStream.Flush();
                fileStream.Close();

            httpWebRequest.ContentLength = memStream.Length;
            Stream requestStream = httpWebRequest.GetRequestStream();
            memStream.Position = 0;
            byte[] tempBuffer = new byte[memStream.Length];

            memStream.Read(tempBuffer, 0, tempBuffer.Length);
            memStream.Close();
            requestStream.Write(tempBuffer, 0, tempBuffer.Length);
                requestStream.Flush();
            requestStream.Close();

                WebResponse webResponse = httpWebRequest.GetResponse();
                Stream stream = webResponse.GetResponseStream();
                StreamReader reader = new StreamReader(stream);

                string var = reader.ReadToEnd();
                httpWebRequest = null;

                return var;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "fail";
            }
        }
    }
}
