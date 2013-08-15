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
        public MovieListing(Movie m)
        {
            this.InitializeComponent();
            this.m = m;
            title.Text = m.Name;
            year.Text = m.Year.ToString();

            if (m.Genres.Length > 1)
            {
                genre.Text = m.Genres[0] + "/" + m.Genres[1];
            }
            else
            {
                genre.Text = m.Genres[0];
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
