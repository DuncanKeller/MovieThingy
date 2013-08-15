﻿using System;
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
        string[] directors;
        string[] genres;
        string[] writers;
        int rating;
        int year;
        string runtime;

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

        public int Year
        {
            get { return year; }
        }

        public Movie(string name)
        {
            this.filename = name;
            string[] parsed = name.Split(' ');
            this.name = name.Substring(0, name.Length - parsed[parsed.Length - 1].Length);
            this.year = Int32.Parse(parsed[parsed.Length - 1].Substring(1, 4));

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

            runtime = Json.GetField("runtime", jsonInfo);
            rating = Int32.Parse(Json.GetField("rating", jsonInfo));
            description = Json.GetField("plot_simple", jsonInfo);
            actors = Json.GetFieldArr("actors", jsonInfo);
            directors = Json.GetFieldArr("directors", jsonInfo);
            genres = Json.GetFieldArr("genres", jsonInfo);
            writers = Json.GetFieldArr("writers", jsonInfo);

            //update(this);
            return;
        }
    }
}
