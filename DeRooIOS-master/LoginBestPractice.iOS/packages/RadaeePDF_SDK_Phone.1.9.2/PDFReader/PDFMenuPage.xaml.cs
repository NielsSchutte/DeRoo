using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using RDPDFLib.pdf;
using System.Collections.ObjectModel;
using System.Text;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml;

namespace PDFReader
{
    public partial class PDFMenuPage : Page
    {
        private string mFileName = String.Empty;
        private PDFDoc m_doc;
        private PDFOutline mOutlineRoot;
        public static PDFDoc ms_doc = null;
        private int mLevel;

        public PDFMenuPage()
        {
            InitializeComponent();
            m_doc = new PDFDoc();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            m_doc = ms_doc;
            ms_doc = null;
            mLevel = 0;
            if( m_doc != null )
                mOutlineRoot = m_doc.GetRootOutline();
            if (mOutlineRoot != null)
                GetMenuList(mOutlineRoot);
            else
            {
                mParentTitle.Text = "No menu data for this document";
                mParentTitle.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ms_doc = null;
            mOutlineRoot = null;
        }
        private void GetMenuList(PDFOutline menuItem)
        { 
            PDFOutline current = menuItem;
            do
            {
                PDFMenuItem item = new PDFMenuItem();
                StringBuilder titleBuilder = new StringBuilder();
                for (int i = 0; i < mLevel; i++)
                    titleBuilder.Append("    ");
                titleBuilder.Append(current.label);
                item.MenuTitle = titleBuilder.ToString();
                item.Dest = current.dest;
                mMenuList.Items.Add(item);
                PDFOutline child = current.GetChild();
                if (child != null)
                {
                    mLevel++;
                    GetMenuList(child);
                }
            } while ((current = current.GetNext()) != null);
            mLevel--;
        }

        private void OnMenuListItemTapped(object sender, TappedRoutedEventArgs e)
        {
            TextBlock item = sender as TextBlock;
            string dest = item.Tag.ToString();
            m_doc = null;
            if (!int.TryParse(dest, out RadaeePDFPage.pageNum))
                RadaeePDFPage.pageNum = 0;
            Frame.GoBack();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            mOutlineRoot = null;
            m_doc = null;
        }

        private void OnBackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mOutlineRoot = null;
            m_doc = null;
        }
    }

    public class PDFMenuItem
    {
        private String _menuTitle;
        public String MenuTitle
        {
            get 
            {
                return _menuTitle;
            }
            set 
            {
                _menuTitle = value;
            }
        }

        private int _dest;
        public int Dest
        {
            get
            {
                return _dest;
            }
            set
            {
                _dest = value;
            }
        }
    }
}