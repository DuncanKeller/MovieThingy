using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using MyToolkit.Multimedia;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Utilities;
using Newtonsoft.Json.Linq;

namespace MovieClient
{
    static class Network
    {
        public static MessageWebSocket messageWebSocket;
        public static DataWriter messageWriter;
        static string url_template = "http://ajax.googleapis.com/ajax/services/search/web?v=2.0&rsz=2&safe=active&q={0}&start={1}";


        private static async Task<string> GetVidID(string m, string y)
        {
            string query = m + " " + y + " trailer youtube";
            Uri searchUri = new Uri(string.Format(url_template, query, 0));
            string pageResult = await MakeWebRequest(searchUri);
            JObject o = (JObject)JsonConvert.DeserializeObject(pageResult);

            JArray jsonResult = (JArray)o["responseData"]["results"];
            string videoResult = (string)(jsonResult[0] as JObject)["unescapedUrl"];
            string[] parsedvideoResult = videoResult.Split('=');
            string id = parsedvideoResult[parsedvideoResult.Length - 1];
            return id;
        }

        private static async Task<string> MakeWebRequest(Uri uri)
        {
           HttpClient http = new System.Net.Http.HttpClient();
           HttpResponseMessage response = await http.GetAsync(uri);
           return await response.Content.ReadAsStringAsync();
        }

        public static async Task<Uri> GetVideo(Movie m)
        {
            string id = await GetVidID(m.Name, m.Year.ToString());
            var url = await YouTube.GetVideoUriAsync(id, YouTubeQuality.Quality480P);
            return url.Uri;
        }

        public static async void SendMessage(string message)
        {
            try
            {
                string address = "192.168.1.1";
                MessageWebSocket webSocket = Network.messageWebSocket;

                if (webSocket == null)
                {
                    Uri server = new Uri(address);

                    webSocket = new MessageWebSocket();

                    // callbacks
                    webSocket.Control.MessageType = SocketMessageType.Utf8;
                    webSocket.MessageReceived += MessageReceived;
                    webSocket.Closed += Closed;

                    // connect
                    await webSocket.ConnectAsync(server);
                    Network.messageWebSocket = webSocket;
                    Network.messageWriter = new DataWriter(webSocket.OutputStream);
                }

                Network.messageWriter.WriteString(message);
                await Network.messageWriter.StoreAsync();

            }
            catch (Exception e) // For debugging
            {
                string error = e.Message;
            }

        }

        private static void MessageReceived(MessageWebSocket sender, MessageWebSocketMessageReceivedEventArgs args)
        {

        }

        private static void Closed(IWebSocket sender, WebSocketClosedEventArgs args)
        {
            if (Network.messageWebSocket != null)
            {
                Network.messageWebSocket.Dispose();
            }

        }
    }
}
