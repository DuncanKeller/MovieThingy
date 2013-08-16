using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MovieClient
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RemotePage : Page
    {
        bool paused = false;
        Page page;
        bool muted = false;

        public RemotePage(Page p)
        {
            page = p;
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void Grid_SizeChanged_1(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width > e.PreviousSize.Width
                && e.PreviousSize.Width > 0)
            {
                Window.Current.Content = page;
            }
        }

        private void play_pause_Click(object sender, RoutedEventArgs e)
        {
            if (paused)
            {
                Network.SendMessage("play");
            }
            else
            {
                Network.SendMessage("pause");
            }
            paused = !paused;
        }

        private void stop_Click(object sender, RoutedEventArgs e)
        {
            Network.SendMessage("stop");
        }

        private void rewind_Click(object sender, RoutedEventArgs e)
        {
            Network.SendMessage("rewind");
        }

        private void fastForward_Click(object sender, RoutedEventArgs e)
        {
            Network.SendMessage("forward");
        }

        private void mute_Click(object sender, RoutedEventArgs e)
        {
            Network.SendMessage("mute");

            muted = !muted;
            if (muted)
            {
                //mute.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(100, 255, 255, 255));
                mute.Content = "unmute";
                //mute.Foreground.Opacity = 0.5;
                volume.Opacity = 0.3;
            }
            else
            {
                //mute.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255));
                mute.Content = "mute";
                //mute.Foreground.Opacity = 1;
                volume.Opacity = 1;
            }
        }

        private void volume_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Network.SendMessage("volume>" + e.NewValue.ToString());
        }
    }
}
