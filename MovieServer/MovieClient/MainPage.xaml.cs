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
using System.Threading.Tasks;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MovieClient
{
    public delegate void UpdateMovie(Movie m);

    public sealed partial class MainPage : Page
    {
        StorageFolder directory;

        public MainPage()
        {
            this.InitializeComponent();

            if (directory != null)
            {
                Init();
            }
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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Movie m = new Movie("Moon (2009)", UpdateMovie);
        }

        public void UpdateMovie(Movie m)
        {
            testText.Text = m.Name;
            testText2.Text = m.Description;
            testText3.Text = m.Actors[0] + ", " + m.Actors[1] + ", " + m.Actors[2];
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

        
    }
}
