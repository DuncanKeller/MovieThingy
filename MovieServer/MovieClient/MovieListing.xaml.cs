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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MovieClient
{
    public sealed partial class MovieListing : UserControl
    {
        Movie m;

        public Movie Movie
        {
            get { return m; }
        }

        public MovieListing(Movie m)
        {
            this.InitializeComponent();
            this.m = m;
            title.Text = m.Name;
            year.Text = m.Year.ToString();

            genre.Text = "";
            for (int i = 0; i < m.Genres.Length; i++)
            {
                genre.Text += m.Genres[i];
                if (i < m.Genres.Length - 1)
                {
                    genre.Text += ", ";
                }
            }

            rating.Text = "";
            int castedRating = (int)Math.Round(m.Rating);
            for (int i = 0; i < castedRating; i++)
            {
                rating.Text += "*";
            }

        }
    }
}
