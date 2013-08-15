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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MovieClient
{
    public class Movie
    {
        string filename;
        string name;
        string description;
        string[] actors;
        int year;
        string genre;
        int rating;

        JObject jsonInfo;
        UpdateMovie update;

        public string Name
        {
            get { return name; }
        }

        public string Description
        {
            get { return description; }
        }

        public string[] Actors
        {
            get { return actors; }
        }

        public Movie(string name, UpdateMovie update)
        {
            this.filename = name;
            this.name = name.Split(' ')[0];
            this.year = Int32.Parse(name.Split(' ')[1].Substring(1, 4));
            this.update = update;

            GetIMBDInfo();
        }

        public async Task GetIMBDInfo()
        {
            try
            {
                if (Json.entries.ContainsKey(filename))
                {
                    jsonInfo = Json.entries[filename];
                }
                else
                {
                    string parameters = "q=" + name + "&" + "year=" + year;
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

                    jsonInfo = Json.ParseJson(System.Text.Encoding.UTF8.GetString(jsonArr.ToArray(), 0, jsonArr.ToArray().Length));
                    jsonInfo.Add("filename", (JToken)filename);
                    Json.newEntries.Add(filename, jsonInfo);
                }
            }
            catch (WebException e)
            {
                WebExceptionStatus error = e.Status;

                throw e;
            }

           
            description = Json.GetDescription(jsonInfo);
            actors = Json.GetActors(jsonInfo);

            update(this);
            return;
        }
    }
}
