using System;
using System.Collections.Generic;
using RDPDFLib.pdf;
using RDPDFLib.view;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using System.ComponentModel;
using System.IO;
using Windows.UI.Xaml.Input;
using Windows.System;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using RDPDFLib.util;
using Windows.Phone.UI.Input;
using Windows.Storage.AccessCache;
namespace PDFReader
{
    public delegate void OnReaderCloseHandle(PDFView.PDFPos pos);

    public partial class RadaeePDFPage : Page, PDFView.PDFViewListener
    {
        private string mFileName;
        private static PDFDoc m_doc = null;
        private PDFView m_view = null;

        private AppBarButton mViewModeBtn = null;
        private AppBarButton mAnnotModeBtn = null;
        private AppBarButton mViewMenuBtn = null;
        private AppBarButton mViewSettingsBtn = null;
        private AppBarButton mViewInfoBtn = null;
        private AppBarButton mSelectTextBtn = null;

        private AppBarButton mHorzViewMode = null;
        private AppBarButton mVertViewMode = null;
        private AppBarButton mDualViewMode = null;

        private AppBarButton mAnnotInk = null;
        private AppBarButton mAnnotRect = null;
        private AppBarButton mAnnotElllipse = null;
        private AppBarButton mAnnotText = null;

        private AppBarButton mAnnotPerformItem = null;
        private AppBarButton mAnnotRemoveItem = null;
        private AppBarButton mAnnotCancelItem = null;

        private bool mIsInRootAppBar;

        private int mCurrentViewMode;

        private PDFAnnot mAnnot = null;

        public static event OnReaderCloseHandle OnReaderClose;

        private PDFStreamWrap mStreamWrap = null;

        private string mFileToken = string.Empty;

        private string mSearchKey = string.Empty;
        private TextAnnotControl mTextAnnotDialog;
        private bool mIsDocModified;

        private TextSelectionControl mTextSelectionControl;

        private Canvas mAnnotCanvas;

        private PasswordControl mPasswordControl = null;
        private IRandomAccessStream mStream = null;


        static public int pageNum = -1;

        private enum Status
        {
            sta_none = 0,
            sta_moving = 1,
            sta_zooming = 2,
            sta_select = 3,
            sta_ink = 4,
            sta_rect = 5,
            sta_ellipse = 6,
            sta_text = 7,
        }

        private Status mStatus;

        public RadaeePDFPage()
        {
            try
            {
                InitializeComponent();
                mCurrentViewMode = 0;

                //ApplicationBar = new ApplicationBar();

                // Do NOT remove this line.
                //Or SizeChanged event of PDFView will be fired unexpectly by the platform.
                //That might interupt annotation operation process
                mAppBar.Opacity = 0.9;

                InitApplicationBar();
                mAppBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                mSearchPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                mPageStack.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            catch (Exception e)
            {
                string msg = e.Message;
            }
        }

        private void InitApplicationBar()
        {
            mAppBar.PrimaryCommands.Clear();
            mAppBar.SecondaryCommands.Clear();

            if (mViewModeBtn == null)
            {
                mViewModeBtn = new AppBarButton();
                mViewModeBtn.Label = "View Mode";
                BitmapIcon icon = new BitmapIcon();
                icon.UriSource = new Uri("ms-appx:///Assets/img/icon_view.png", UriKind.Absolute);
                mViewModeBtn.Icon = icon;
                mViewModeBtn.Click += new RoutedEventHandler(OnMenuButtonClick);
            }
            mAppBar.PrimaryCommands.Add(mViewModeBtn);

            if (mAnnotModeBtn == null)
            {
                mAnnotModeBtn = new AppBarButton();
                BitmapIcon icon = new BitmapIcon();
                icon.UriSource = new Uri("ms-appx:///Assets/img/icon_annot.png", UriKind.Absolute);
                mAnnotModeBtn.Icon = icon;
                mAnnotModeBtn.Label = "Add Annotation";
                mAnnotModeBtn.Click += new RoutedEventHandler(OnMenuButtonClick);
            }
            mAppBar.PrimaryCommands.Add(mAnnotModeBtn);

            if (mViewMenuBtn == null)
            {
                mViewMenuBtn = new AppBarButton();
                BitmapIcon icon = new BitmapIcon();
                icon.UriSource = new Uri("ms-appx:///Assets/img/icon_menu.png", UriKind.Absolute);
                mViewMenuBtn.Icon = icon;
                mViewMenuBtn.Label = "Menu";
                mViewMenuBtn.Click += new RoutedEventHandler(OnMenuButtonClick);
            }
            mAppBar.PrimaryCommands.Add(mViewMenuBtn);

            if (mViewSettingsBtn == null)
            {
                mViewSettingsBtn = new AppBarButton();
                BitmapIcon icon = new BitmapIcon();
                icon.UriSource = new Uri("ms-appx:///Assets/img/icon_setting.png", UriKind.Absolute);
                mViewSettingsBtn.Icon = icon;
                mViewSettingsBtn.Label = "settings";
                mViewSettingsBtn.Click += new RoutedEventHandler(OnMenuButtonClick);
            }
            mAppBar.PrimaryCommands.Add(mViewSettingsBtn);

            if (mSelectTextBtn == null)
            {
                mSelectTextBtn = new AppBarButton();
                mSelectTextBtn.Label = "Select text";
                mSelectTextBtn.Click += new RoutedEventHandler(OnMenuButtonClick);
            }
            mAppBar.SecondaryCommands.Add(mSelectTextBtn);

            if (mViewInfoBtn == null)
            {
                mViewInfoBtn = new AppBarButton();
                mViewInfoBtn.Label = "About";
                mViewInfoBtn.Click += new RoutedEventHandler(OnMenuButtonClick);
            }
            mAppBar.SecondaryCommands.Add(mViewInfoBtn);

            mIsInRootAppBar = true;
        }

        private void OnMenuButtonClick(Object sender, RoutedEventArgs e)
        {
            AppBarButton button = sender as AppBarButton;

            if (button != null)
            {
                if (button.Equals(mViewModeBtn))
                    SetViewModeAppBar();
                else if (button.Equals(mAnnotModeBtn))
                    SetAnnotModeAppBar();
                else if (button.Equals(mViewMenuBtn))
                {
                    PDFMenuPage.ms_doc = m_doc;//pass a Document object to the page.
                    Frame.Navigate(typeof(PDFMenuPage));
                }
                else if (button.Equals(mViewSettingsBtn))
                {
                    Frame.Navigate(typeof(PDFSettingsPage));
                }
                else if (button.Equals(mSelectTextBtn))
                {
                    //m_view.vSetSelStatus(true);
                    m_view.vSelStart();
                    m_view.vSetLock(3);
                }
                else if (button.Equals(mViewInfoBtn))
                {
                    pageNum = m_view.vGetPos(0, 0).pageno;
                    Frame.Navigate(typeof(AboutPage));
                }
            }
        }

        private void OnViewModeButtonClick(Object sender, RoutedEventArgs e)
        {
            AppBarButton button = sender as AppBarButton;
            if (button.Equals(this.mVertViewMode) && mCurrentViewMode != 0)
            {
                set_view(0);
                mCurrentViewMode = 0;
            }
            else if (button.Equals(this.mHorzViewMode) && mCurrentViewMode != 1)
            {
                set_view(1);
                mCurrentViewMode = 1;
            }
            else if (button.Equals(this.mDualViewMode) && mCurrentViewMode != 3)
            {
                set_view(3);
                mCurrentViewMode = 3;
            }

        }

        private void SetAnnotModeAppBar()
        {
            mAppBar.PrimaryCommands.Clear();
            mAppBar.SecondaryCommands.Clear();

            if (mAnnotInk == null)
            {
                mAnnotInk = new AppBarButton();
                BitmapIcon icon = new BitmapIcon();
                icon.UriSource = new Uri("ms-appx:///Assets/img/icon_annot_ink.png", UriKind.Absolute);
                mAnnotInk.Icon = icon;
                mAnnotInk.Label = "doodle";
                mAnnotInk.Click += new RoutedEventHandler(OnAnnotButtonClick);
            }
            mAppBar.PrimaryCommands.Add(mAnnotInk);

            if (mAnnotRect == null)
            {
                mAnnotRect = new AppBarButton();
                BitmapIcon icon = new BitmapIcon();
                icon.UriSource = new Uri("ms-appx:///Assets/img/icon_annot_rect.png", UriKind.Absolute);
                mAnnotRect.Icon = icon;
                mAnnotRect.Label = "doodle";
                mAnnotRect.Click += new RoutedEventHandler(OnAnnotButtonClick);
            }
            mAppBar.PrimaryCommands.Add(mAnnotRect);

            if (mAnnotElllipse == null)
            {
                mAnnotElllipse = new AppBarButton();
                BitmapIcon icon = new BitmapIcon();
                icon.UriSource = new Uri("ms-appx:///Assets/img/icon_annot_ellipse.png", UriKind.Absolute);
                mAnnotElllipse.Icon = icon;
                mAnnotElllipse.Label = "doodle";
                mAnnotElllipse.Click += new RoutedEventHandler(OnAnnotButtonClick);
            }
            mAppBar.PrimaryCommands.Add(mAnnotElllipse);

            if (mAnnotText == null)
            {
                mAnnotText = new AppBarButton();
                BitmapIcon icon = new BitmapIcon();
                icon.UriSource = new Uri("ms-appx:///Assets/img/icon_annot_text.png", UriKind.Absolute);
                mAnnotText.Icon = icon;
                mAnnotText.Label = "doodle";
                mAnnotText.Click += new RoutedEventHandler(OnAnnotButtonClick);
            }
            mAppBar.PrimaryCommands.Add(mAnnotText);

            mIsInRootAppBar = false;
        }

        private void OnAnnotButtonClick(Object sender, RoutedEventArgs e)
        {
            AppBarButton button = sender as AppBarButton;
            if (mStatus != Status.sta_none)
            {
                AnnotEnd(false);
                return;
            }
            BitmapIcon icon = button.Icon as BitmapIcon;
            icon.UriSource = new Uri("ms-appx:///Assets/img/icon_annot_done.png", UriKind.Absolute);

            if (button.Equals(mAnnotInk))
            {
                AnnotStart(Status.sta_ink);
            }
            else if (button.Equals(this.mAnnotRect))
            {
                AnnotStart(Status.sta_rect);
            }
            else if (button.Equals(this.mAnnotElllipse))
            {
                AnnotStart(Status.sta_ellipse);
            }
            else if (button.Equals(this.mAnnotText))
            {
                AnnotStart(Status.sta_text);
            }
        }

        private void OnAnnotItemClick(Object sender, RoutedEventArgs e)
        {
            AppBarButton button = sender as AppBarButton;

            if (button.Equals(mAnnotCancelItem))
            {
                m_view.vAnnotEnd();
            }
            else if (button.Equals(mAnnotPerformItem))
            {
                m_view.vAnnotPerform();
            }
            else if (button.Equals(mAnnotRemoveItem))
            {
                m_view.vAnnotRemove();
                mIsDocModified = true;
            }

            InitApplicationBar();
        }

        private void AnnotStart(Status status)
        {
            mStatus = status;
            switch (status)
            {
                case Status.sta_ink:
                    m_view.vInkStart();
                    break;
                case Status.sta_rect:
                    m_view.vRectStart();
                    break;
                case Status.sta_ellipse:
                    m_view.vEllipseStart();
                    break;
                case Status.sta_text:
                    m_view.vNoteStart();
                    break;
                default:
                    break;
            }
            m_view.vSetLock(3);
            mPDFView.Children.Add(mAnnotCanvas);
        }

        private void AnnotEnd(bool cancel)
        {
            if (mIsDocModified == false)
                mIsDocModified = !cancel;
            if (mStatus == Status.sta_ink)
            {
                BitmapIcon icon = mAnnotInk.Icon as BitmapIcon;
                icon.UriSource = new Uri("ms-appx:///Assets/img/icon_annot_ink.png", UriKind.Absolute);
                //this.mAnnotInk.IconUri = new Uri("Assets/img/icon_annot_ink.png", UriKind.Relative);
                if (cancel)
                    m_view.vInkCancel();
                else
                    m_view.vInkEnd();
            }
            else if (mStatus == Status.sta_rect)
            {
                BitmapIcon icon = mAnnotRect.Icon as BitmapIcon;
                icon.UriSource = new Uri("ms-appx:///Assets/img/icon_annot_rect.png", UriKind.Absolute);
                if (cancel)
                    m_view.vRectCancel();
                else
                    m_view.vRectEnd();
            }
            else if (mStatus == Status.sta_ellipse)
            {
                BitmapIcon icon = mAnnotElllipse.Icon as BitmapIcon;
                icon.UriSource = new Uri("ms-appx:///Assets/img/icon_annot_ellipse.png", UriKind.Absolute);
                if (cancel)
                    m_view.vEllipseCancel();
                else
                    m_view.vEllipseEnd();
            }
            else if (mStatus == Status.sta_text)
            {
                BitmapIcon icon = mAnnotText.Icon as BitmapIcon;
                icon.UriSource = new Uri("ms-appx:///Assets/img/icon_annot_text.png", UriKind.Absolute);
                m_view.vNoteEnd();
            }
            mStatus = Status.sta_none;
            m_view.vSetLock(0);
            mPDFView.Children.Remove(mAnnotCanvas);

            RDPDFLib.view.PDFView.PDFPos pos = m_view.vGetPos(0, 0);
            PDFVPage page = m_view.vGetPage(pos.pageno);
            if (page != null)
                m_view.vRenderSync(page);
        }

        private void SetViewModeAppBar()
        {
            mAppBar.PrimaryCommands.Clear();
            mAppBar.SecondaryCommands.Clear();

            if (mHorzViewMode == null)
            {
                mHorzViewMode = new AppBarButton();
                BitmapIcon icon = new BitmapIcon();
                icon.UriSource = new Uri("ms-appx:///Assets/img/icon_view_option_horz.png", UriKind.Absolute);
                mHorzViewMode.Icon = icon;
                mHorzViewMode.Label = "horizontal view";
                mHorzViewMode.Click += new RoutedEventHandler(OnViewModeButtonClick);
            }
            mAppBar.PrimaryCommands.Add(mHorzViewMode);

            if (mVertViewMode == null)
            {
                mVertViewMode = new AppBarButton();
                BitmapIcon icon = new BitmapIcon();
                icon.UriSource = new Uri("ms-appx:///Assets/img/icon_view_option_vert.png", UriKind.Absolute);
                mVertViewMode.Icon = icon;
                mVertViewMode.Label = "horizontal view";
                mVertViewMode.Click += new RoutedEventHandler(OnViewModeButtonClick);
            }
            mAppBar.PrimaryCommands.Add(mVertViewMode);

            if (mDualViewMode == null)
            {
                mDualViewMode = new AppBarButton();
                BitmapIcon icon = new BitmapIcon();
                icon.UriSource = new Uri("ms-appx:///Assets/img/icon_view_option_single_pro.png", UriKind.Absolute);
                mDualViewMode.Icon = icon;
                mDualViewMode.Label = "horizontal view";
                mDualViewMode.Click += new RoutedEventHandler(OnViewModeButtonClick);
            }
            mAppBar.PrimaryCommands.Add(mDualViewMode);

            mIsInRootAppBar = false;
        }

        private void OnBackPress(object sender, BackPressedEventArgs e)
        {
            if (mStatus != Status.sta_none)
            {
                AnnotEnd(true);
                SetAnnotModeAppBar();
                e.Handled = true;
            }
            else if (!mIsInRootAppBar)
            {
                InitApplicationBar();
                e.Handled = true;
            }
            else
            {
                if (m_doc != null)
                {
                    if (mIsDocModified)
                    {
                        SaveDocControl saveDocControl = new SaveDocControl();
                        saveDocControl.onSaveDialogClose += OnSaveDialogClosed;
                        saveDocControl.show();
                        e.Handled = true;
                    }
                    else
                    {
                        try
                        {
                            PDFView.PDFPos pos = m_view.vGetPos(0, 0);
                            if (OnReaderClose != null)
                                OnReaderClose(pos);
                            m_view.vClose();
                            m_doc.Close();
                            if (mStream != null)
                            {
                                mStream.Dispose();
                                mStream = null;
                            }
                            if (mStreamWrap != null)
                            {
                                mStreamWrap.Close();
                                mStreamWrap = null;
                            }
                            m_view = null;
                            m_doc = null;
                            if(Frame.CanGoBack)
                            {
                                e.Handled = true;
                                Frame.GoBack();
                            }
                        }
                        catch (Exception ex)
                        {
                            String msg = ex.Message;
                        }
                    }
                }
            }
        }

        private void OnSaveDialogClosed(int btnCode)
        {
            if (btnCode == SaveDocControl.CODE_CANCEL)
                return;
            if (btnCode == SaveDocControl.CODE_OK)
                m_doc.Save();
            PDFView.PDFPos pos = m_view.vGetPos(0, 0);
            if (OnReaderClose != null)
                OnReaderClose(pos);
            m_view.vClose();
            m_doc.Close();
            if (mStream != null)
            {
                mStream.Dispose();
                mStream = null;
            }
            if (mStreamWrap != null)
            {
                mStreamWrap.Close();
                mStreamWrap = null;
            }
            m_view = null;
            m_doc = null;
            mStream = null;
            mStreamWrap = null;
            if (Frame.CanGoBack)
                Frame.GoBack();
            else
            {
                Application.Current.Exit();
            }

        }

        private void LayoutRoot_Unloaded(object sender, RoutedEventArgs e)
        {
            if (m_doc != null)
            {
                PDFView.PDFPos pos = m_view.vGetPos(0, 0);
                if (OnReaderClose != null)
                    OnReaderClose(pos);
                m_view.vClose();
                m_doc.Close();
                if (mStream != null) mStream.Dispose();
                if (mStreamWrap != null) mStreamWrap.Close();
                m_view = null;
                m_doc = null;
                mStream = null;
                mStreamWrap = null;
            }
        }
        // Assign the path or token value, depending on how the page was launched.

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += OnBackPress;
            mAnnotCanvas = new Canvas();

            if (m_doc != null)
            {
                if (pageNum != -1)
                {
                    if (m_view == null)
                    {
                        m_view = new PDFViewVert(mPDFView);
                        m_view.vOpen(m_doc, 4, 0xFFCCCCCC, this);
                    }
                    m_view.vGotoPage(pageNum);
                    pageNum = -1;
                }
                return;
            }
            m_doc = new PDFDoc();
            if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(PDFSettingsPage.VIEW_MODE))
            {
                int mode = Convert.ToInt32(Windows.Storage.ApplicationData.Current.LocalSettings.Values[PDFSettingsPage.VIEW_MODE]);
                switch (mode)
                {
                    case 0:
                        m_view = new PDFViewVert(mPDFView);
                        break;
                    case 1:
                        m_view = new PDFViewHorz(mPDFView);
                        break;
                    case 3:
                        m_view = new PDFViewHorz(mPDFView);
                        break;
                }
            }
            else
                m_view = new PDFViewVert(mPDFView);

            var file = e.Parameter;
            if (file != null && file.GetType().Equals(typeof(StorageFile)))
            {
                await ProcessExternalFile(file as StorageFile);
            }
            else if (file != null && file.GetType().Equals(typeof(string)))
            {
                string path = file as string;
                if (path.StartsWith("*"))
                    await ProcessAssetPDFFile(path.Remove(0, 1));
                else
                    await ProcessListFile(file as string);
            }
        }

        protected override async void OnNavigatedFrom(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed -= OnBackPress;
        }

        //private string mOneDrivePath;
        private async Task ProcessListFile(string token)
        {
            try
            {
                StorageFile file = await StorageApplicationPermissions.MostRecentlyUsedList.GetFileAsync(token);
                mStream = await file.OpenAsync(FileAccessMode.ReadWrite);
                PDF_ERROR error = m_doc.Open(mStream, "");
                if (error == PDF_ERROR.err_password)
                {
                    //mFileToken = pSDFilePath;
                    if (mPasswordControl == null)
                    {
                        mPasswordControl = new PasswordControl();
                        mPasswordControl.OnPasswordAvailable += OnPasswordAvailabel;
                    }
                    mPasswordControl.Show();
                }
                else if (error == PDF_ERROR.err_ok)
                {
                    if (!m_doc.CanSave)
                    {
                        mAnnotModeBtn.IsEnabled = false;
                        mSelectTextBtn.IsEnabled = false;
                    }
                    m_view.vOpen(m_doc, 4, 0xFFCCCCCC, this);
                    if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(token + "_page") && Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(token + "_x") && Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(token + "_y"))
                    {
                        PDFView.PDFPos pos = new PDFView.PDFPos();
                        pos.pageno = Convert.ToInt32(Windows.Storage.ApplicationData.Current.LocalSettings.Values[token + "_page"]);
                        pos.x = (float)Windows.Storage.ApplicationData.Current.LocalSettings.Values[token + "_x"];
                        pos.y = (float)Windows.Storage.ApplicationData.Current.LocalSettings.Values[token + "_y"];
                        m_view.vSetPos(pos, 0, 0);
                    }
                }
                else
                {
                    if (m_doc != null)
                    {
                        m_doc.Close();
                        m_doc = null;
                    }
                    if (Frame.CanGoBack)
                        Frame.GoBack();
                    else
                    {
                        Application.Current.Exit();
                    }
                }
            }
            catch(Exception e)
            {
                string msg = e.Message;
                if (m_doc != null)
                {
                    m_doc.Close();
                    m_doc = null;
                }
                if (Frame.CanGoBack)
                    Frame.GoBack();
                else
                {
                    Application.Current.Exit();
                }
            }
        }

        private async Task ProcessExternalFile(StorageFile file)
        {
            try
            {
                mStream = await file.OpenAsync(FileAccessMode.ReadWrite);
                PDF_ERROR error = m_doc.Open(mStream, "");
                if (error == PDF_ERROR.err_password)
                {
                    //mFileToken = pSDFilePath;
                    if (mPasswordControl == null)
                    {
                        mPasswordControl = new PasswordControl();
                        mPasswordControl.OnPasswordAvailable += OnPasswordAvailabel;
                    }
                    mPasswordControl.Show();
                }
                else if (error == PDF_ERROR.err_ok)
                {
                    if (!m_doc.CanSave)
                    {
                        mAnnotModeBtn.IsEnabled = false;
                        mSelectTextBtn.IsEnabled = false;
                    }
                    m_view.vOpen(m_doc, 4, 0xFFCCCCCC, this);
                    if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(MainPage.mFileToken + "_page") && Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(MainPage.mFileToken + "_x") && Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(MainPage.mFileToken + "_y"))
                    {
                        PDFView.PDFPos pos = new PDFView.PDFPos();
                        pos.pageno = Convert.ToInt32(Windows.Storage.ApplicationData.Current.LocalSettings.Values[MainPage.mFileToken + "_page"]);
                        pos.x = (float)Windows.Storage.ApplicationData.Current.LocalSettings.Values[MainPage.mFileToken + "_x"];
                        pos.y = (float)Windows.Storage.ApplicationData.Current.LocalSettings.Values[MainPage.mFileToken + "_y"];
                        m_view.vSetPos(pos, 0, 0);
                    }
                    if (m_doc.GetRootOutline() == null)
                        mViewMenuBtn.IsEnabled = false;
                    mPageDisplay.Text = "/" + m_doc.PageCount.ToString();
                    mPageStack.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
                else
                {
                    if (m_doc != null)
                    {
                        m_doc.Close();
                        m_doc = null;
                    }
                    if (Frame.CanGoBack)
                        Frame.GoBack();
                    else
                    {
                        Application.Current.Exit();
                    }
                }
            }
            catch (Exception e)
            {
                // The route is not present on the SD card.
                //MessageBox.Show("That route is missing on your SD card.");
                string msg = e.Message;
            }
        }
        public async Task ProcessAssetPDFFile(string file)
        {
            StorageFolder folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets");
            StorageFile sfile = await folder.GetFileAsync(file);
            mStream = await sfile.OpenAsync(FileAccessMode.Read);
            m_doc.Open(mStream, "");
            m_view.vOpen(m_doc, 4, 0xFFCCCCCC, this);
        }
        public async Task ProcessInternalPDFFile(string file)
        {

            IStorageFolder routesFolder = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFolderAsync("ebook", CreationCollisionOption.OpenIfExists);
            try
            {
                StorageFile internalFile = await routesFolder.GetFileAsync(file);
                mFileName = internalFile.Name;
                Task<IRandomAccessStream> task1 = internalFile.OpenAsync(FileAccessMode.ReadWrite).AsTask();
                mStream = task1.Result;

                PDF_ERROR error = m_doc.Open(mStream, "");

                if (error == PDF_ERROR.err_password)
                {
                    mFileToken = file;
                    if (mPasswordControl == null)
                    {
                        mPasswordControl = new PasswordControl();
                        mPasswordControl.OnPasswordAvailable += OnPasswordAvailabel;
                    }
                    mPasswordControl.Show();
                }
                else if (error == PDF_ERROR.err_ok)
                {
                    //if (!m_doc.CanSave)
                    //{
                    //    mAnnotModeBtn.IsEnabled = false;
                    //    mSelectTextBtn.IsEnabled = false;
                    //}
                    if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(file + "_page") && Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(file + "_x") && Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(file + "_y"))
                    {
                        PDFView.PDFPos pos = new PDFView.PDFPos();
                        pos.pageno = Convert.ToInt32(Windows.Storage.ApplicationData.Current.LocalSettings.Values[file + "_page"]);
                        pos.x = (float)Windows.Storage.ApplicationData.Current.LocalSettings.Values[file + "_x"];
                        pos.y = (float)Windows.Storage.ApplicationData.Current.LocalSettings.Values[file + "_y"];
                        m_view.vSetPos(pos, 0, 0);
                    }
                    m_view.vOpen(m_doc, 4, 0xFFCCCCCC, this);
                    //if (m_doc.GetRootOutline() == null)
                    //    mViewMenuBtn.IsEnabled = false;
                    mPageDisplay.Text = "/" + m_doc.PageCount.ToString();
                    mPageStack.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }

                else
                {
                    if (Frame.CanGoBack)
                        Frame.GoBack();
                    else
                    {
                        Application.Current.Exit();
                    }
                }
            }
            catch (Exception ex)
            {
                if (Frame.CanGoBack)
                    Frame.GoBack();
                return;
            }
        }

        private void OnPasswordAvailabel(string password)
        {
            if (password == null)
            {
                mPasswordControl.Dismiss();
                if (mStream != null) mStream.Dispose();
                if (mStreamWrap != null) mStreamWrap.Close();
                if (Frame.CanGoBack)
                    Frame.GoBack();
                else
                {
                    Application.Current.Exit();
                }
                return;
            }
            PDF_ERROR error = m_doc.Open(mStream, password);
            if (error == PDF_ERROR.err_ok)
            {
                if (!m_doc.CanSave)
                {
                    //mAnnotModeBtn.IsEnabled = false;
                    //mSelectTextBtn.IsEnabled = false;
                }
                if (!mFileToken.Equals(string.Empty) && Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(mFileToken + "_page") && Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(mFileToken + "_x") && Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(mFileToken + "_y"))
                {
                    PDFView.PDFPos pos = new PDFView.PDFPos();
                    pos.pageno = Convert.ToInt32(Windows.Storage.ApplicationData.Current.LocalSettings.Values[mFileToken + "_page"]);
                    pos.x = (float)Windows.Storage.ApplicationData.Current.LocalSettings.Values[mFileToken + "_x"];
                    pos.y = (float)Windows.Storage.ApplicationData.Current.LocalSettings.Values[mFileToken + "_y"];
                    m_view.vSetPos(pos, 0, 0);
                }
                mPasswordControl.Dismiss();
                m_view.vOpen(m_doc, 4, 0xFFCCCCCC, this);
                mPageDisplay.Text = "/" + m_doc.PageCount.ToString();
                mPageStack.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            else if (error == PDF_ERROR.err_password)
                mPasswordControl.ShowHint();
        }

        // Process a route from a file association.
        public async Task ProcessAssociatePDFFile(string fileToken)
        {

        }

        private void OnCloseDialog(String subject, String content, bool cancel, bool edit)
        {
            if (cancel)
            {
                m_view.vNoteRemoveLast();
                return;
            }
            if (mAnnot == null)
            {
                int index = -1;
                RDPDFLib.view.PDFView.PDFPos pos = m_view.vGetPos(0, 0);
                PDFVPage vpage = m_view.vGetPage(pos.pageno);
                if (vpage != null)
                {
                    PDFPage page = vpage.GetPage();
                    if (page != null)
                    {
                        index = page.AnnotCount;
                        if (index > 0)
                            mAnnot = page.GetAnnot(index - 1);
                        if (mAnnot == null)
                            return;
                    }
                }
            }
            mAnnot.PopupSubject = subject;
            mAnnot.PopupText = content;
            if (mTextAnnotDialog != null)
            {
                mTextAnnotDialog.dismiss();
                mTextAnnotDialog = null;
            }
        }

        private void OnTextSelectionDialogClose(bool cancel, int type)
        {
            if (!cancel && type != -1)
            {
                if (type != 5)
                {
                    uint color = 0;
                    if (type == 0)
                        color = Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(PDFSettingsPage.HIGHLIGHT_COLOR) ? (uint)Windows.Storage.ApplicationData.Current.LocalSettings.Values[PDFSettingsPage.HIGHLIGHT_COLOR] : PDFSettingsPage.DefaultHighlightColor;
                    else if (type == 1)
                        color = Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(PDFSettingsPage.UNDER_LINE_COLOR) ? (uint)Windows.Storage.ApplicationData.Current.LocalSettings.Values[PDFSettingsPage.UNDER_LINE_COLOR] : PDFSettingsPage.DefaultUnderLineColor;
                    else if (type == 2)
                        color = Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(PDFSettingsPage.STRIK_OUT_COLOR) ? (uint)Windows.Storage.ApplicationData.Current.LocalSettings.Values[PDFSettingsPage.STRIK_OUT_COLOR] : PDFSettingsPage.DefaultStrikoutColor;
                    else if (type == 3)
                        color = Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(PDFSettingsPage.HIGHLIGHT_COLOR_2) ? (uint)Windows.Storage.ApplicationData.Current.LocalSettings.Values[PDFSettingsPage.HIGHLIGHT_COLOR_2] : PDFSettingsPage.DefaultHighlightColor2;
                    else if (type == 4)
                        color = Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(PDFSettingsPage.SQUIGGLY_UNDER_LINE_COLOR) ? (uint)Windows.Storage.ApplicationData.Current.LocalSettings.Values[PDFSettingsPage.SQUIGGLY_UNDER_LINE_COLOR] : PDFSettingsPage.DefaultSquigglyUnderLineColor;
                    m_view.vSelMarkup(color, type);
                    mIsDocModified = true;
                }
                //else
                //{
                //    //copy selected text to clipboard
                //    string selection = m_view.vSelGetText();
                //    System.Windows. Clipboard.SetText(selection);
                //}

            }
            mTextSelectionControl.Dismiss();
            mTextSelectionControl = null;
            mStatus = Status.sta_none;
            m_view.vSelEnd();
            m_view.vSetLock(0);
        }

        private void set_view(int mode)
        {
            PDFView.PDFPos pos = m_view.vGetPos((int)mPDFView.Width / 2, (int)mPDFView.Height / 2);
            m_view.vClose();
            switch (mode)
            {
                case 0:
                    m_view = new PDFViewVert(mPDFView);
                    break;
                case 1:
                    m_view = new PDFViewHorz(mPDFView);
                    ((PDFViewHorz)m_view).vSetDirection(false);
                    break;
                case 3:
                    m_view = new PDFViewHorz(mPDFView);
                    ((PDFViewHorz)m_view).vSetDirection(true);
                    break;
                default:
                    m_view = new PDFViewVert(mPDFView);
                    break;
            }
            m_view.vOpen(m_doc, 4, 0xFFCCCCCC, this);
            mStatus = Status.sta_none;
            m_view.vSetPos(pos, (int)mPDFView.Width / 2, (int)mPDFView.Height / 2);
            //todo reset other UI or inner status.
            InitApplicationBar();
        }

        private void SearchButtonTapped(object sender, TappedRoutedEventArgs e)
        {
            string searchKey = mSearchText.Text;
            if (searchKey.Length == 0)
            {
                m_view.vFindEnd();
                return;
            }
            bool matchCase = false;
            if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(PDFSettingsPage.SEARCH_MATCH_CASE))
                matchCase = Convert.ToBoolean(Windows.Storage.ApplicationData.Current.LocalSettings.Values[PDFSettingsPage.SEARCH_MATCH_CASE]);

            bool matchWholeWord = true;
            if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(PDFSettingsPage.SEARCH_WHOLE_WORD))
                matchCase = Convert.ToBoolean(Windows.Storage.ApplicationData.Current.LocalSettings.Values[PDFSettingsPage.SEARCH_WHOLE_WORD]);
            if (!searchKey.Equals(mSearchKey))
            {
                mSearchKey = searchKey;
                m_view.vFindStart(mSearchKey, matchCase, matchWholeWord);
            }
            Image icon = sender as Image;
            if (icon.Equals(mSearchPrev))
            {
                m_view.vFind(-1);
            }
            else if (icon.Equals(mSearchNext))
            {
                m_view.vFind(1);
            }
            else if (icon.Equals(mSearchCancel))
            {
                m_view.vFindEnd();
            }
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            /* for testing
            m_view.vZoom(1.5f);
            PDFView.PDFPos pos = new PDFView.PDFPos();
            pos.pageno = 10;
            pos.x = 0;
            pos.y = 0;
            m_view.vSetPos(pos, 0, 0);
            */
            //m_view.vInkStart();
        }

        public void OnPDFViewLoaded()
        {
        }

        /**
            * fired when position changed.
            * @param pos position in PDF coordinate.<br/>
            * mostly you can do something with pos:<br/>
            * --use vGetPage(pos.pageno) to get more details and do coordinate conversion.<br/>
            * --determine whether pageno has changed.<br/>
            * --enumerate all displayed pages by check coordinates.<br/>
            */
        public void OnPDFPageChanged(int pageno)
        {
            mPageInput.Text = (++pageno).ToString();
        }

        /**
            * fired when single tapped.
            * @param x x coordinate
            * @param y y coordinate
            * @return true, if process it, or skipped.
            */
        public Boolean OnPDFSingleTapped(float x, float y)
        {
            if (mIsInRootAppBar)
            {
                //mAppBar.IsOpen = !mAppBar.IsOpen;
                mAppBar.Visibility = (mAppBar.Visibility == Windows.UI.Xaml.Visibility.Collapsed) ? Windows.UI.Xaml.Visibility.Visible : Windows.UI.Xaml.Visibility.Collapsed;
            }
            if (mSearchPanel.Visibility == Windows.UI.Xaml.Visibility.Visible)
            {
                mSearchPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                mPageStack.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            else
            {
                mSearchPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
                PDFView.PDFPos pos = m_view.vGetPos(0, 0);
                mPageInput.Text = (++pos.pageno).ToString();
                mPageStack.Visibility = Windows.UI.Xaml.Visibility.Visible;

            }
            return false;
        }

        public void OnPDFLongPressed(float x, float y)
        {
        }

        /**
            * fired when long pressed.
            * @param x x coordinate
            * @param y y coordinate
            */
        public void OnPDFFound(Boolean found)
        {
        }

        /**
            * fired when a page displayed.
            * @param vpage
            */
        public void OnPDFPageDisplayed(Canvas canvas, PDFVPage vpage)
        {

        }

        /**
            * fired when selecting.
            * @param canvas Canvas object to draw.
            * @param rect1 first char's location, in Canvas coordinate.
            * @param rect2 last char's location, in Canvas coordinate.
            */
        public void OnPDFSelecting(Canvas canvas, PDFRect rect1, PDFRect rect2)
        {
        }

        /**
            * fired when text selected.
            */
        public void OnPDFSelected()
        {
            mTextSelectionControl = new TextSelectionControl();
            mTextSelectionControl.OnDialogClose += OnTextSelectionDialogClose;
            mTextSelectionControl.Show();
        }

        public void OnPDFAnnotClicked(PDFPage page, PDFAnnot annot)
        {
            if (annot != null)
            {
                mAppBar.PrimaryCommands.Clear();
                mAppBar.SecondaryCommands.Clear();
                mAppBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                if (mAnnotPerformItem == null)
                {
                    mAnnotPerformItem = new AppBarButton();
                    BitmapIcon icon = new BitmapIcon();
                    icon.UriSource = new Uri("ms-appx:///Assets/img/annot_open.png", UriKind.Absolute);
                    mAnnotPerformItem.Icon = icon;
                    mAnnotPerformItem.Label = "open annotation";
                    mAnnotPerformItem.Click += OnAnnotItemClick;
                }
                mAppBar.PrimaryCommands.Add(mAnnotPerformItem);

                if (mAnnotRemoveItem == null)
                {
                    mAnnotRemoveItem = new AppBarButton();
                    BitmapIcon icon = new BitmapIcon();
                    icon.UriSource = new Uri("ms-appx:///Assets/img/annot_delete.png", UriKind.Absolute);
                    mAnnotRemoveItem.Icon = icon;
                    mAnnotRemoveItem.Label = "delete annotation";
                    mAnnotRemoveItem.Click += OnAnnotItemClick;
                }
                mAppBar.PrimaryCommands.Add(mAnnotRemoveItem);

                if (mAnnotCancelItem == null)
                {
                    mAnnotCancelItem = new AppBarButton();
                    BitmapIcon icon = new BitmapIcon();
                    icon.UriSource = new Uri("ms-appx:///Assets/img/icon_annot_cancel.png", UriKind.Absolute);
                    mAnnotCancelItem.Icon = icon;
                    mAnnotCancelItem.Label = "cancel";
                    mAnnotCancelItem.Click += OnAnnotItemClick;
                }
                mAppBar.PrimaryCommands.Add(mAnnotCancelItem);
                mAppBar.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
        }

        public void OnPDFAnnotEnd()
        {
        }

        public void OnPDFAnnotGoto(int pageno)
        {
            m_view.vGotoPage(pageno);
        }

        public void OnPDFAnnotURI(String uri)
        {
        }

        public void OnPDFAnnotMovie(PDFAnnot annot, String name)
        {
        }

        public void OnPDFAnnotSound(PDFAnnot annot, String name)
        {
        }

        public void OnPDFAnnotPopup(PDFAnnot annot, String subj, String text)
        {
            mAnnot = annot;
            if (mTextAnnotDialog == null)
            {
                mTextAnnotDialog = new TextAnnotControl();
            }
            if (annot != null)
                mTextAnnotDialog.SetContent(annot.PopupSubject, annot.PopupText);
            mTextAnnotDialog.OnCloseDialog += OnCloseDialog;
            mTextAnnotDialog.show();
        }

        private void PhoneApplicationPage_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            try
            {
                if (e.Key == VirtualKey.Enter)
                {
                    int page = Convert.ToInt32(mPageInput.Text);
                    m_view.vGotoPage(--page);
                }
            }
            catch
            {
            }
        }

    }

    class RandomStream : IRandomAccessStream
    {
        Stream internstream;

        public RandomStream(Stream underlyingstream)
        {
            internstream = underlyingstream;
        }

        public IInputStream GetInputStreamAt(ulong position)
        {
            //THANKS Microsoft! This is GREATLY appreciated!
            internstream.Position = (long)position;
            return internstream.AsInputStream();
        }

        public IOutputStream GetOutputStreamAt(ulong position)
        {
            internstream.Position = (long)position;
            return internstream.AsOutputStream();
        }

        public ulong Size
        {
            get
            {
                return (ulong)internstream.Length;
            }
            set
            {
                internstream.SetLength((long)value);
            }
        }

        public bool CanRead
        {
            get { return this.internstream.CanRead; }
        }

        public bool CanWrite
        {
            get { return this.internstream.CanWrite; }
        }

        public IRandomAccessStream CloneStream()
        {
            throw new NotSupportedException();
        }

        public ulong Position
        {
            get { return (ulong)this.internstream.Position; }
        }

        public void Seek(ulong position)
        {
            this.internstream.Seek((long)position, SeekOrigin.Begin);
        }

        public void Dispose()
        {
            this.internstream.Dispose();
        }

        public Windows.Foundation.IAsyncOperationWithProgress<IBuffer, uint> ReadAsync(IBuffer buffer, uint count, InputStreamOptions options)
        {
            return this.GetInputStreamAt(this.Position).ReadAsync(buffer, count, options);
        }

        public Windows.Foundation.IAsyncOperation<bool> FlushAsync()
        {
            return this.GetOutputStreamAt(this.Position).FlushAsync();
        }

        public Windows.Foundation.IAsyncOperationWithProgress<uint, uint> WriteAsync(IBuffer buffer)
        {
            return this.GetOutputStreamAt(this.Position).WriteAsync(buffer);
        }
    }

}