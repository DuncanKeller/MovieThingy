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

//todo
//json library
//convert byte array to string

namespace MovieClient
{
    class Movie
    {
        string name;
        string description;
        string[] tags;
        string genre;
        int rating;

        string jsonInfo = "";

        public async void GetIMBDInfo()
        {
            string parameters = "q=" + name;
            byte[] paramStream = Encoding.UTF8.GetBytes(parameters);

            WebRequest movieRequest = WebRequest.CreateHttp("http://mymovieapi.com/");
            movieRequest.Credentials = CredentialCache.DefaultCredentials;
            movieRequest.Method = "POST";

            Stream requestStream = await movieRequest.GetRequestStreamAsync();
            requestStream.Write(paramStream, 0, paramStream.Length);
            requestStream.Flush();
            requestStream.Dispose();

            WebResponse movieResponse = await movieRequest.GetResponseAsync();
            Stream responseStream = movieResponse.GetResponseStream();
            byte[] jsonArr = new byte[responseStream.Length];
            responseStream.Read(jsonArr, 0, (int)responseStream.Length);
            responseStream.Dispose();
            movieResponse.Dispose();


        }
    }
}
