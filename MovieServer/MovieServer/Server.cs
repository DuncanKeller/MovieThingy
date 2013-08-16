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


namespace MovieServer
{
    class Server
    {

        public Server()
        {
            
        }
        
        public void WaitForConnection()
        {
            while (true)
            {
                MessageWebSocket webSock = new MessageWebSocket();
                webSock.Control.MessageType = SocketMessageType.Utf8; 
                //webSock.MessageReceived += MessageRecieved;
            }
        }

        //public void StartServer(HTTPContext context)
        //{

        //}

        //public void MessageRecieved(MessageWebSocket sender, 
        //    MessageWebSocketMessageReceivedEventArgs args)
        //{
        //    try
        //    {
        //        using (DataReader reader = args.GetDataReader())
        //        {
        //            string command = reader.ReadString(reader.UnconsumedBufferLength);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //}
    }
}
