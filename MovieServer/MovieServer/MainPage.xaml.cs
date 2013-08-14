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

namespace MovieServer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        StorageFolder directory;

        public MainPage()
        {
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

        private async void SetDirectory_Click(object sender, RoutedEventArgs e)
        {
            FolderPicker picker = new FolderPicker();
            picker.CommitButtonText = "yeah this one";
            picker.FileTypeFilter.Add("*");
            picker.ViewMode = PickerViewMode.List;

            directory = await picker.PickSingleFolderAsync();
        }

        public void PlayAMovieYouDonkus()
        {
            
        }

        public bool TypeSupported(string type)
        {
            return type == ".mp4";
        }

        private async Task<Dictionary<string, StorageFile>> GetSubFiles(StorageFolder folder)
        {
            Dictionary<string, StorageFile> returnList = new Dictionary<string, StorageFile>();
            IReadOnlyList<StorageFile> files = await folder.GetFilesAsync();
            foreach (StorageFile file in files)
            {
                if (TypeSupported(file.FileType))
                {
                    returnList.Add(file.Name, file);
                }
            }

            IReadOnlyList<StorageFolder> folders = await folder.GetFoldersAsync();

            if (folders.Count > 0)
            {
                foreach (StorageFolder newFolder in folders)
                {
                    Dictionary<string, StorageFile> newFiles = await GetSubFiles(newFolder);

                    foreach (StorageFile file in newFiles.Values)
                    {
                        returnList.Add(file.Name, file);
                    }
                }
            }

            return returnList;
        }

        public async void FindMovie(string name)
        {
            IReadOnlyDictionary<string, StorageFile> files = await GetSubFiles(directory);

            if (files.ContainsKey(name))
            {
                IRandomAccessStream movieStream = await files[name].OpenAsync(FileAccessMode.Read);
                moviePlaya.SetSource(movieStream, files[name].ContentType);
                moviePlaya.Play();
            }
            else
            {
                throw new Exception("Porkus, this file doesn't exist!");
            }
        }

        private void testButton_Click(object sender, RoutedEventArgs e)
        {
            string testMovie = "DuckTales- Remastered- Giant Bomb Quick Look.mp4";

            FindMovie(testMovie);
        }

        private void moviePlaya_BufferingProgressChanged(object sender, RoutedEventArgs e)
        {
            bufferAmnt.Text = moviePlaya.BufferingProgress + " %";
        }

        private void moviePlaya_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            //bufferAmnt.Text = moviePlaya.CurrentState.ToString();
        }
    }
}
