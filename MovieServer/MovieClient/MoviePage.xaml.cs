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
    public sealed partial class MoviePage : Page
    {
        Movie m;
        Page returnTo;

        public Movie Movie
        {
            get { return m; }
        }

        public MoviePage(Movie m, Page returnTo)
        {
            this.InitializeComponent();
            this.m = m;
            this.returnTo = returnTo;

            name.Text = m.Name;
            year.Text = "year: " + m.Year;
            rating.Text = "rating: ";
            for (int i = 0; i < (int)Math.Round(m.Rating); i++)
            { rating.Text += "*"; }
            genre.Text = "genres: ";
            for (int i = 0; i < m.Genres.Length; i++)
            {
                genre.Text += m.Genres[i];
                if (i < m.Genres.Length - 1)
                {
                    genre.Text += ", ";
                }
            }
            runtime.Text = "runtime: " + m.Runtime;
            director.Text = "director: " + m.Directors[0];
            writer.Text = "writer: " + m.Writers[0];
            actors.Text = "actors: ";
            for (int i = 0; i < m.Actors.Length; i++)
            {
                actors.Text += m.Actors[i];

                if (i > 4)
                { break; }
                else if(i == 4)
                { actors.Text += ", ";}
            }
            plot.Text = m.Description;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            Window.Current.Content = returnTo;
        }
    }
}
