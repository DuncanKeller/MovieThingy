using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Threading;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.Storage;

namespace MovieClient
{
    public class Movie
    {
        string name;
        string description;
        string[] tags;
        string genre;
        int rating;

        string jsonInfo = "";
        UpdateMovie update;

        public string JsonInfo
        {
            get { return jsonInfo; }
        }

        public Movie(string name, UpdateMovie update)
        {
            this.name = name;
            this.update = update;

            GetIMBDInfo();
        }

        public async Task GetIMBDInfo()
        {
            try
            {
                string parameters = "q=" + name;
                byte[] paramStream = Encoding.UTF8.GetBytes(parameters);

                WebRequest movieRequest = WebRequest.CreateHttp("http://mymovieapi.com/" + "?" + parameters);
                movieRequest.Credentials = CredentialCache.DefaultCredentials;
                movieRequest.Method = "GET";
                /*
                Stream requestStream = await movieRequest.GetRequestStreamAsync();
                requestStream.Write(paramStream, 0, paramStream.Length);
                requestStream.Flush();
                //requestStream.Dispose();
                */
                WebResponse movieResponse = await movieRequest.GetResponseAsync();
                Stream responseStream = movieResponse.GetResponseStream();
                List<byte> jsonArr = new List<byte>();
                while (true)
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead = responseStream.Read(buffer, 0, buffer.Length);
                    jsonArr.AddRange(buffer.Take(bytesRead));
                    if (bytesRead < buffer.Length)
                    {
                        break;
                    }
                }
                responseStream.Dispose();
                movieResponse.Dispose();

                jsonInfo = System.Text.Encoding.UTF8.GetString(jsonArr.ToArray(), 0, jsonArr.ToArray().Length);
            }
            catch (WebException e)
            {
                WebExceptionStatus error = e.Status;

                throw e;
            }

            update(this);
            return;
        }
    }
}
