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

namespace MovieClient
{
    static class Network
    {
        public static MessageWebSocket messageWebSocket;
        public static DataWriter messageWriter;


        private static async void SendMessage(string message)
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
