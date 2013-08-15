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


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MovieClient
{
    public delegate void UpdateMovie(Movie m);

    public sealed partial class MainPage : Page
    {
        StorageFolder directory;
        List<Movie> movies = new List<Movie>();
        List<Movie> filteredMovies = new List<Movie>();

        string filterTitle = "";

        public MainPage()
        {
            this.InitializeComponent();

            if (directory != null)
            {
                Init();
            }
            
            IAsyncAction filterThread = ThreadPool.RunAsync(FilterMovies);
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


        private void FilterMovies(IAsyncAction action)
        {
            // add movies to the filtered list if they meet the conditions
            while (true)
            {
                if (filterTitle != string.Empty)
                {
                    lock (filteredMovies)
                    {
                        List<Movie> removeMe = new List<Movie>();

                        foreach (Movie m in filteredMovies)
                        {
                            if (!m.Name.ToLower().Contains(filterTitle.ToLower()))
                            {
                                removeMe.Add(m);
                            }
                        }

                        foreach (Movie m in movies)
                        {
                            if (!filteredMovies.Contains(m))
                            {
                                if (m.Name.ToLower().Contains(filterTitle.ToLower()))
                                {
                                    filteredMovies.Add(m);
                                }
                            }
                        }

                        foreach (Movie m in removeMe)
                        {
                            filteredMovies.Remove(m);
                        }
                    }
                }
            }
        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {

            filterTitle = title.Text;
            testFilteredList.Text = "";
            lock (filteredMovies)
            {
                foreach (Movie m in filteredMovies)
                {
                    testFilteredList.Text += m.Name + "(" + m.Year + ")";
                    if (m != filteredMovies[filteredMovies.Count - 1])
                    {
                        testFilteredList.Text += ",\r\n";
                    }
                }
            }
        }

        private void TextBox_GotFocus_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
