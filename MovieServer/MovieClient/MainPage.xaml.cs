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
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Storage.Pickers;
using Windows.Storage.Pickers.Provider;
using System.Threading;
using System.Threading.Tasks;
using Windows.System.Threading;
using Windows.System.Threading.Core;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MovieClient
{
    public delegate void UpdateMovie(Movie m);
    public delegate void SortType();

    public sealed partial class MainPage : Page
    {
        bool requests = true;
        SolidColorBrush selectedColor;

        StorageFolder directory;
        List<Movie> movies = new List<Movie>();
        List<Movie> filteredMovies = new List<Movie>();
        SortType sortType;

        string filterTitle = "";
        string filterActors = "";
        string filterDirectors = "";
        string filterGenres = "";


        public MainPage()
        {
            this.InitializeComponent();

            sortType = SortByTitle;

            if (directory != null)
            {
                Init();
            }

            selectedColor = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 150, 200, 150));
            
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void Init()
        {
            Json.Init(directory);
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            IReadOnlyList<string> files = await GetSubFiles(directory);

            foreach (string movie in files)
            {
                Movie m = new Movie(movie);
                movies.Add(m);
            }
        }

        public void UpdateMovie(Movie m)
        {
            
        }

        private async void folderButton_Click(object sender, RoutedEventArgs e)
        {
            FolderPicker picker = new FolderPicker();
            picker.CommitButtonText = "yeah this one";
            picker.FileTypeFilter.Add("*");
            picker.ViewMode = PickerViewMode.List;

            directory = await picker.PickSingleFolderAsync();

            Init();
        }

        public bool TypeSupported(string type)
        {
            return type == ".mp4";
        }

        private async Task<List<string>> GetSubFiles(StorageFolder folder)
        {
            List<string> returnList = new List<string>();
            IReadOnlyList<StorageFile> files = await folder.GetFilesAsync();
            foreach (StorageFile file in files)
            {
                if (TypeSupported(file.FileType))
                {
                    returnList.Add(file.Name);
                }
            }

            IReadOnlyList<StorageFolder> folders = await folder.GetFoldersAsync();

            if (folders.Count > 0)
            {
                foreach (StorageFolder newFolder in folders)
                {
                    List<string> newFiles = await GetSubFiles(newFolder);

                    foreach (string file in newFiles)
                    {
                        returnList.Add(file);
                    }
                }
            }

            return returnList;
        }

        private void AddFilter(string filterType, string filter, ref List<Movie> toAdd)
        {
            if (filter != string.Empty)
            {
                foreach (Movie m in movies)
                {
                    if (!filteredMovies.Contains(m) &&
                        !toAdd.Contains(m))
                    {
                        if (Movie.PropertyIsArray(filterType))
                        {
                            foreach (string property in m.GetPropertyArray(filterType))
                            {
                                if (property.ToLower().Contains(filter.ToLower()))
                                {
                                    toAdd.Add(m);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            if (m.GetProperty(filterType).ToLower().Contains(filter.ToLower()))
                            {
                                toAdd.Add(m);
                            }
                        }
                    }
                }
            }
        }

        private void RemoveFilter(string filterType, string filter, ref List<Movie> toRemove)
        {
            if (filter != string.Empty)
            {
                foreach (Movie m in filteredMovies)
                {
                    if (!toRemove.Contains(m))
                    {
                        if (Movie.PropertyIsArray(filterType))
                        {
                            bool noMatches = true;
                            foreach (string property in m.GetPropertyArray(filterType))
                            {
                                if (property.ToLower().Contains(filter.ToLower()))
                                {
                                    noMatches = false;
                                    break;
                                }
                            }
                            if (noMatches)
                            { toRemove.Add(m); }
                        }
                        else
                        {
                            if (!m.GetProperty(filterType).ToLower().Contains(filter.ToLower()))
                            {
                                toRemove.Add(m);
                            }
                        }
                    }
                }
            }
        }

        private async void FilterMovies(IAsyncAction action)
        {
            // add movies to the filtered list if they meet the conditions
            lock (filteredMovies)
            {
                List<Movie> toRemove = new List<Movie>();
                List<Movie> toAdd = new List<Movie>();

                AddFilter("name", filterTitle, ref toAdd);
                AddFilter("actors", filterActors, ref toAdd);
                AddFilter("directors", filterDirectors, ref toAdd);
                AddFilter("genres", filterGenres, ref toAdd);

                foreach (Movie m in toAdd)
                {
                    filteredMovies.Add(m);
                }

                RemoveFilter("name", filterTitle, ref toRemove);
                RemoveFilter("actors", filterActors, ref toRemove);
                RemoveFilter("directors", filterDirectors, ref toRemove);
                RemoveFilter("genres", filterGenres, ref toRemove);

                foreach (Movie m in toRemove)
                {
                    filteredMovies.Remove(m);
                }

            }
            
            return;
        }

        private async void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            filterTitle = title.Text;
            await ThreadPool.RunAsync(FilterMovies);
            RefreshList();
        }

        private async void actors_TextChanged(object sender, TextChangedEventArgs e)
        {
            filterActors = actors.Text;
            await ThreadPool.RunAsync(FilterMovies);
            RefreshList();
        }

        private async void directors_TextChanged(object sender, TextChangedEventArgs e)
        {
            filterDirectors = directors.Text;
            await ThreadPool.RunAsync(FilterMovies);
            RefreshList();
        }

        private async void genres_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            filterGenres = (genres.SelectedItem as ComboBoxItem).Content.ToString();
            await ThreadPool.RunAsync(FilterMovies);
            RefreshList();
        }

        public async void RefreshList()
        {
            movieList.Items.Clear();
            lock (filteredMovies)
            {
                sortType();

                foreach (Movie m in filteredMovies)
                {
                    movieList.Items.Add(new MovieListing(m));
                }
            }
        }

        private void TextBox_GotFocus_1(object sender, RoutedEventArgs e)
        {

        }

        private void sortName_Click(object sender, RoutedEventArgs e)
        {
            ResetButtonColor();
            sortName.Background = selectedColor;

            sortType = SortByTitle;
            RefreshList();
        }

        private void sortYear_Click(object sender, RoutedEventArgs e)
        {
            ResetButtonColor();
            sortYear.Background = selectedColor;

            sortType = SortByYear;
            RefreshList();
        }

        private void sortRating_Click(object sender, RoutedEventArgs e)
        {
            ResetButtonColor();
            sortRating.Background = selectedColor;

            sortType = SortByRating;
            RefreshList();
        }

        private void ResetButtonColor()
        {
            sortName.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(0, 0, 0, 0));
            sortYear.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(0, 0, 0, 0));
            sortRating.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(0, 0, 0, 0));
        }

        private void SortByTitle()
        {
            filteredMovies.Sort(Movie.SortByTitle);
        }

        private void SortByYear()
        {
            filteredMovies.Sort(Movie.SortByYear);
        }

        private void SortByRating()
        {
            filteredMovies.Sort(Movie.SortByRating);
        }

        
        
    }
}
