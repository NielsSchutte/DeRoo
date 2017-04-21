using System;
using System.Collections.Generic;
using System.Linq;
//using System.Windows.Controls;
//using System.Windows.Navigation;
//using Microsoft.Phone.Controls;
using System.Threading.Tasks;
using Windows.Storage;
//using Microsoft.Phone.Storage;
//using Microsoft.Live;
//using Microsoft.Live.Controls;
using RDPDFLib.view;
//using System.IO.IsolatedStorage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.Storage.Pickers;
using Windows.ApplicationModel.Activation;
using Windows.Storage.AccessCache;
using Windows.UI.Xaml.Navigation;

namespace PDFReader
{
    public class FileItem
    {
        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        private string _token;
        public string Token
        {
            get
            {
                return _token;
            }
            set
            {
                _token = value;
            }
        }
    }

    public partial class MainPage : Page, IFileOpenPickerContinuable
    {
        public static string mFileToken = string.Empty;

        public static MainPage Current;

        private List<FileItem> mFileListData;

        public MainPage()
        {
            InitializeComponent();
            Current = this;
            RadaeePDFPage.OnReaderClose += OnReaderClose;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (mFileListData == null)
                mFileListData = new List<FileItem>();

            foreach (AccessListEntry entry in StorageApplicationPermissions.MostRecentlyUsedList.Entries)
            {
                StorageFile file = await StorageApplicationPermissions.MostRecentlyUsedList.GetFileAsync(entry.Token);
                if (file != null)
                {
                    FileItem item = new FileItem();
                    item.Name = file.Name;
                    item.Token = entry.Token;
                    mFileListData.Add(item);
                }
            }
            mFileList.ItemsSource = mFileListData;
        }

        private void OnReaderClose(PDFView.PDFPos pos)
        {
            if (!mFileToken.Equals(string.Empty))
            {
                if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(mFileToken + "_page"))
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values[mFileToken + "_page"] = pos.pageno;
                else
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values.Add(mFileToken + "_page", pos.pageno);

                if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(mFileToken + "_x"))
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values[mFileToken + "_x"] = pos.x;
                else
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values.Add(mFileToken + "_x", pos.x);

                if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(mFileToken + "_y"))
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values[mFileToken + "_y"] = pos.y;
                else
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values.Add(mFileToken + "_y", pos.y);
            }
        }

        private void OnFileListItemTapped(object sender, RoutedEventArgs e)
        {
            TextBlock panel = sender as TextBlock;
            FileItem item = panel.DataContext as FileItem;
            mFileToken = item.Token;

            Frame.Navigate(typeof(RadaeePDFPage), item.Token);
        }

        private void OnSDFileListItemTapped(object sender, RoutedEventArgs e)
        {

        }

        private void OnBackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //todo: free and destroy all objects.
        }
        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            //todo: free and destroy all objects.
        }
        public void ContinueFileOpenPicker(FileOpenPickerContinuationEventArgs args)
        {
            if (args.Files.Count == 0)
                return;
            StorageFile file = args.Files.First() as StorageFile;
            if (StorageApplicationPermissions.MostRecentlyUsedList.Entries.Count < 15)
            {
                mFileToken = StorageApplicationPermissions.MostRecentlyUsedList.Add(file);
            }
            else
            {
                AccessListEntry entry = StorageApplicationPermissions.MostRecentlyUsedList.Entries[0];
                StorageApplicationPermissions.MostRecentlyUsedList.Remove(entry.Token);
                if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(entry.Token + "_page") && Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(entry.Token + "_x") && Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(entry.Token + "_y"))
                {
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values.Remove(entry.Token + "_page");
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values.Remove(entry.Token + "_x");
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values.Remove(entry.Token + "_y");
                }
                mFileToken = StorageApplicationPermissions.MostRecentlyUsedList.Add(file);
            }
            this.Frame.Navigate(typeof(RadaeePDFPage), file);
        }

        private void mBrowseBtn_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.List;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".pdf");
            openPicker.FileTypeFilter.Add(".PDF");
            openPicker.PickSingleFileAndContinue();
        }

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    this.Frame.Navigate(typeof(RadaeePDFPage), "*test.pdf");
        //}
    }
}