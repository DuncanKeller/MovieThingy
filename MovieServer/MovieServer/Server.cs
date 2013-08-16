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
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;


namespace MovieServer
{
    class Server
    {
        StreamSocketListener listener;
        MediaElement player;
        MainPage page;

        public Server(MediaElement player, MainPage page)
        {
            this.player = player;
            this.page = page;
            Init();
        }

        public async void Init()
        {
            listener = new StreamSocketListener();
            listener.ConnectionReceived += Connected;
            CoreApplication.Properties.Add("listener", listener);

            try
            {
                await listener.BindServiceNameAsync("moviePlayer");
            }
            catch (Exception e)
            {
                throw e;
            }


        }

        private async void Connected(StreamSocketListener sender,
            StreamSocketListenerConnectionReceivedEventArgs args)
        {
            DataReader reader = new DataReader(args.Socket.InputStream);

            try
            {
                while (true)
                {
                    // read string length
                    uint sizeFieldCount = await reader.LoadAsync(sizeof(uint));
                    if (sizeFieldCount != sizeof(uint))
                    {
                       return;
                    }

                    // read string
                    uint stringLength = reader.ReadUInt32();
                    uint actualStringLength = await reader.LoadAsync(stringLength);
                    if (stringLength != actualStringLength)
                    {
                        return;
                    }
                    string message = reader.ReadString(actualStringLength);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private async void RunCommand(string command)
        {
            if (command.Contains("src"))
            {
                string[] args = command.Split('>');
                page.PlayAMovieYouDonkus(args[1]);
            }
            else if (command.Contains("play"))
            {
                player.Play();
            }
            else if (command.Contains("stop"))
            {
                player.Stop();
            }
            else if (command.Contains("pause"))
            {
                player.Pause();
            }
            else if (command.Contains("mute"))
            {
                player.IsMuted = !player.IsMuted;
            }
            else if (command.Contains("volume"))
            {
                string[] args = command.Split('>');
                int vol = Int32.Parse(args[1]);
                player.Volume = (double)(vol / 100.0);
            }
            else if (command.Contains("forward"))
            {
                player.Position.Add(TimeSpan.FromSeconds(5));
            }
            else if (command.Contains("rewind"))
            {
                player.Position.Subtract(TimeSpan.FromSeconds(5));
            }
        }

    }
}
