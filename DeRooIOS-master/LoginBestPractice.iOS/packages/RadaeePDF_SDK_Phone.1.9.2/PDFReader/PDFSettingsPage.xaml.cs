using RDPDFLib.pdf;
using RDPDFLib.view;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace PDFReader
{
    public partial class PDFSettingsPage : Page
    {

        public static string RENDER_MODE = "RENDER_MODE";
        public static string VIEW_MODE = "VIEW_MODE";
        public static string TEXT_RTL = "TEXT_RTL";
        public static string INK_COLOR = "INK_COLOR";
        public static string INK_WIDTH = "INK_WIDTH";
        public static string RECT_COLOR = "RECT_COLOR";
        public static string RECT_WIDTH = "RECT_WIDTH";
        public static string ELLIPSE_COLOR = "ELLIPSE_COLOR";
        public static string ELLIPSE_WIDTH = "ELLIPSE_WIDTH";
        public static string UNDER_LINE_COLOR = "UNDER_LINE_COLOR";
        public static string STRIK_OUT_COLOR = "STRIK_OUT_COLOR";
        public static string HIGHLIGHT_COLOR = "HIGHLIGHT_COLOR";
        public static string HIGHLIGHT_COLOR_2 = "HIGHLIGHT_COLOR_2";
        public static string SQUIGGLY_UNDER_LINE_COLOR = "SQUIGGLY_UNDER_LINE_COLOR";
        public static string SEARCH_MATCH_CASE = "SEARCH_MATCH_CASE";
        public static string SEARCH_WHOLE_WORD = "SEARCH_WHOLE_WORD";

        public const PDF_RENDER_MODE DefaultRenderMode = PDF_RENDER_MODE.mode_normal;
        public const int DefaultViewMode = 0;//0: Vertical mode; 1: Horizontal mode; 3:Dual page mode
        public const bool DefaultTextRTL = false;
        public const float DefaultInkWidth = 4;
        public const uint DefaultInkColor = 0xFFFF0000;
        public const uint DefaultRectColor = 0xFF00FF00;
        public const float DefaultRectWidth = 5;
        public const uint DefaultEllipseColor = 0xFF0000FF;
        public const float DefaultEllipseWidth = 6;
        public const bool DefaultSearchMatchCase = false;
        public const bool DefaultSearchWholeWrod = true;
        //0xFFD9DE01
        public const uint DefaultHighlightColor = 0xFFD9DE01;
        public const uint DefaultHighlightColor2 = 0xFFD9DE01;
        public const uint DefaultStrikoutColor = 0xFFD9DE01;
        public const uint DefaultUnderLineColor = 0xFFD9DE01;
        public const uint DefaultSquigglyUnderLineColor = 0xFFD9DE01;

        private List<string> mRenderModeList;
        private List<string> mViewModeList;

        private ColorPickerControl mColorPickerControl = null;

        private int mMarkupType;

        public PDFSettingsPage()
        {
            InitializeComponent();

            mRenderModeList = new List<string>();
            mRenderModeList.Add("Poor");
            mRenderModeList.Add("Normal");
            mRenderModeList.Add("Best");

            mViewModeList = new List<string>();
            mViewModeList.Add("Vertical Mode");
            mViewModeList.Add("Horizontal Mode");
            mViewModeList.Add("Dual Page Mode");

            mRenderModeListPicker.ItemsSource = mRenderModeList;
            if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(RENDER_MODE))
            {
                mRenderModeListPicker.SelectedIndex = (int)Windows.Storage.ApplicationData.Current.LocalSettings.Values[RENDER_MODE];
                //switch ((int)Windows.Storage.ApplicationData.Current.LocalSettings.Values[RENDER_MODE])
                //{
                //    case 2:
                //        mRenderModeListPicker.SelectedIndex = 2;
                //        break;
                //    case PDF_RENDER_MODE.mode_normal:
                //        mRenderModeListPicker.SelectedIndex = 1;
                //        break;
                //    case PDF_RENDER_MODE.mode_poor:
                //        mRenderModeListPicker.SelectedIndex = 0;
                //        break;
                //}
            }
            mRenderModeListPicker.SelectionChanged += mRenderModeListPicker_SelectionChanged;

            mViewModeListPicker.ItemsSource = mViewModeList;
            mViewModeListPicker.SelectionChanged += mViewModeListPicker_SelectionChanged;
            if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(VIEW_MODE))
            {
                switch ((int)Windows.Storage.ApplicationData.Current.LocalSettings.Values[VIEW_MODE])
                {
                    case 0:
                        mViewModeListPicker.SelectedIndex = 0;
                        break;
                    case 1:
                        mViewModeListPicker.SelectedIndex = 1;
                        break;
                    case 3:
                        mViewModeListPicker.SelectedIndex = 2;
                        break;
                }
            }

            uint currentInkColor;
            if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(INK_COLOR))
                currentInkColor = (uint)Windows.Storage.ApplicationData.Current.LocalSettings.Values[INK_COLOR];
            else
                currentInkColor = PDFView.inkColor;
            Color clr = new Color();
            clr.R = (byte)((currentInkColor & 0x00FF0000) >> 16);
            clr.G = (byte)((currentInkColor & 0x0000FF00) >> 8);
            clr.B = (byte)(currentInkColor & 0x000000FF);
            clr.A = (byte)((currentInkColor & 0xFF000000) >> 24);
            mInkColor.Fill = new SolidColorBrush(clr);

            uint currentRectColor;
            if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(RECT_COLOR))
                currentRectColor = (uint)Windows.Storage.ApplicationData.Current.LocalSettings.Values[RECT_COLOR];
            else
                currentRectColor = PDFView.rectColor;
            clr = new Color();
            clr.R = (byte)((currentRectColor & 0x00FF0000) >> 16);
            clr.G = (byte)((currentRectColor & 0x0000FF00) >> 8);
            clr.B = (byte)(currentRectColor & 0x000000FF);
            clr.A = (byte)((currentRectColor & 0xFF000000) >> 24);
            mRectColor.Fill = new SolidColorBrush(clr);

            uint currentEllipseColor;
            if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(ELLIPSE_COLOR))
                currentEllipseColor = (uint)Windows.Storage.ApplicationData.Current.LocalSettings.Values[ELLIPSE_COLOR];
            else
                currentEllipseColor = PDFView.ovalColor;
            clr = new Color();
            clr.R = (byte)((currentEllipseColor & 0x00FF0000) >> 16);
            clr.G = (byte)((currentEllipseColor & 0x0000FF00) >> 8);
            clr.B = (byte)(currentEllipseColor & 0x000000FF);
            clr.A = (byte)((currentEllipseColor & 0xFF000000) >> 24);
            mEllipseColor.Fill = new SolidColorBrush(clr);

            uint currentHighlightColor;
            if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(HIGHLIGHT_COLOR))
                currentHighlightColor = (uint)Windows.Storage.ApplicationData.Current.LocalSettings.Values[HIGHLIGHT_COLOR];
            else
                currentHighlightColor = DefaultHighlightColor;
            clr = new Color();
            clr.R = (byte)((currentHighlightColor & 0x00FF0000) >> 16);
            clr.G = (byte)((currentHighlightColor & 0x0000FF00) >> 8);
            clr.B = (byte)(currentHighlightColor & 0x000000FF);
            clr.A = (byte)((currentHighlightColor & 0xFF000000) >> 24);
            mHighlightColor.Fill = new SolidColorBrush(clr);

            uint currentHighlightColor2;
            if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(HIGHLIGHT_COLOR_2))
                currentHighlightColor2 = (uint)Windows.Storage.ApplicationData.Current.LocalSettings.Values[HIGHLIGHT_COLOR_2];
            else
                currentHighlightColor2 = DefaultHighlightColor2;
            clr = new Color();
            clr.R = (byte)((currentHighlightColor2 & 0x00FF0000) >> 16);
            clr.G = (byte)((currentHighlightColor2 & 0x0000FF00) >> 8);
            clr.B = (byte)(currentHighlightColor2 & 0x000000FF);
            clr.A = (byte)((currentHighlightColor2 & 0xFF000000) >> 24);
            mHighlightColor2.Fill = new SolidColorBrush(clr);

            uint currentStrikoutColor;
            if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(STRIK_OUT_COLOR))
                currentStrikoutColor = (uint)Windows.Storage.ApplicationData.Current.LocalSettings.Values[STRIK_OUT_COLOR];
            else
                currentStrikoutColor = DefaultStrikoutColor;
            clr = new Color();
            clr.R = (byte)((currentStrikoutColor & 0x00FF0000) >> 16);
            clr.G = (byte)((currentStrikoutColor & 0x0000FF00) >> 8);
            clr.B = (byte)(currentStrikoutColor & 0x000000FF);
            clr.A = (byte)((currentStrikoutColor & 0xFF000000) >> 24);
            mStrikoutColor.Fill = new SolidColorBrush(clr);

            uint currentUnderlineColor;
            if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(UNDER_LINE_COLOR))
                currentUnderlineColor = (uint)Windows.Storage.ApplicationData.Current.LocalSettings.Values[UNDER_LINE_COLOR];
            else
                currentUnderlineColor = DefaultUnderLineColor;
            clr = new Color();
            clr.R = (byte)((currentUnderlineColor & 0x00FF0000) >> 16);
            clr.G = (byte)((currentUnderlineColor & 0x0000FF00) >> 8);
            clr.B = (byte)(currentUnderlineColor & 0x000000FF);
            clr.A = (byte)((currentUnderlineColor & 0xFF000000) >> 24);
            mUnderlineColor.Fill = new SolidColorBrush(clr);

            uint currentSquigglyUnderlineColor;
            if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(SQUIGGLY_UNDER_LINE_COLOR))
                currentSquigglyUnderlineColor = (uint)Windows.Storage.ApplicationData.Current.LocalSettings.Values[SQUIGGLY_UNDER_LINE_COLOR];
            else
                currentSquigglyUnderlineColor = DefaultSquigglyUnderLineColor;
            clr = new Color();
            clr.R = (byte)((currentSquigglyUnderlineColor & 0x00FF0000) >> 16);
            clr.G = (byte)((currentSquigglyUnderlineColor & 0x0000FF00) >> 8);
            clr.B = (byte)(currentSquigglyUnderlineColor & 0x000000FF);
            clr.A = (byte)((currentSquigglyUnderlineColor & 0xFF000000) >> 24);
            mSquigglyUnderlineColor.Fill = new SolidColorBrush(clr);

            if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(INK_WIDTH))
                mInkWidth.Text = Windows.Storage.ApplicationData.Current.LocalSettings.Values[INK_WIDTH].ToString();
            else
                mInkWidth.Text = DefaultInkWidth.ToString();

            if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(RECT_WIDTH))
                mRectWidth.Text = Windows.Storage.ApplicationData.Current.LocalSettings.Values[RECT_WIDTH].ToString();
            else
                mRectWidth.Text = DefaultRectWidth.ToString();

            if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(ELLIPSE_WIDTH))
                mEllilpseWidth.Text = Windows.Storage.ApplicationData.Current.LocalSettings.Values[ELLIPSE_WIDTH].ToString();
            else
                mEllilpseWidth.Text = DefaultEllipseWidth.ToString();

            if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(SEARCH_MATCH_CASE))
                mMatchCase.IsChecked = (bool)Windows.Storage.ApplicationData.Current.LocalSettings.Values[SEARCH_MATCH_CASE];
            else
                mMatchCase.IsChecked = DefaultSearchMatchCase;

            if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(SEARCH_WHOLE_WORD))
                mWholeword.IsChecked = (bool)Windows.Storage.ApplicationData.Current.LocalSettings.Values[SEARCH_WHOLE_WORD];
            else
                mWholeword.IsChecked = DefaultSearchWholeWrod;

            mMarkupType = -1;
        }

        void mViewModeListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            String item;
            if (e.AddedItems.Count > 0)
            {
                item = e.AddedItems[0] as string;
                int mode;
                switch (item)
                {
                    case "Vertical Mode":
                        mode = 0;
                        break;
                    case "Horizontal Mode":
                        mode = 1;
                        break;
                    case "Dual Page Mode":
                        mode = 3;
                        break;
                    default:
                        mode = 0;
                        break;
                }
                if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(VIEW_MODE))
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values[VIEW_MODE] = mode;
                else
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values.Add(VIEW_MODE, mode);
            }
        }

        void mRenderModeListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            String item;
            if (e.AddedItems.Count > 0)
            {
                item = e.AddedItems[0] as string;
                int mode;
                switch (item)
                {
                    case "Poor":
                        mode = 0;
                        PDFView.renderQuality = PDF_RENDER_MODE.mode_poor;
                        break;
                    case "Normal":
                        mode = 1;
                        PDFView.renderQuality = PDF_RENDER_MODE.mode_normal;
                        break;
                    case "Best":
                        mode = 2;
                        PDFView.renderQuality = PDF_RENDER_MODE.mode_best;
                        break;
                    default:
                        mode = 1;
                        PDFView.renderQuality = PDF_RENDER_MODE.mode_normal;
                        break;
                }
                if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(RENDER_MODE))
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values[RENDER_MODE] = mode;
                else
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values.Add(RENDER_MODE, mode);
            }
        }

        private void mRTLCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(TEXT_RTL))
                Windows.Storage.ApplicationData.Current.LocalSettings.Values[TEXT_RTL] = mRTLCheckBox.IsChecked;
            else
                Windows.Storage.ApplicationData.Current.LocalSettings.Values.Add(TEXT_RTL, mRTLCheckBox.IsChecked);
        }

        private void mInkWidth_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            float value;
            if (float.TryParse(mInkWidth.Text, out value))
            {
                if (value < 0.5)
                    value = 0.5f;
                else if (value > 8)
                    value = 8;
                if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(INK_WIDTH))
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values[INK_WIDTH] = value;
                else
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values.Add(INK_WIDTH, value);
                PDFView.inkWidth = value;
            }
        }

        private void mMatchCase_Click(object sender, RoutedEventArgs e)
        {
            if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(SEARCH_MATCH_CASE))
                Windows.Storage.ApplicationData.Current.LocalSettings.Values[SEARCH_MATCH_CASE] = mMatchCase.IsChecked;
            else
                Windows.Storage.ApplicationData.Current.LocalSettings.Values.Add(SEARCH_MATCH_CASE, mMatchCase.IsChecked);
        }

        private void mWholeword_Click(object sender, RoutedEventArgs e)
        {
            if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(SEARCH_WHOLE_WORD))
                Windows.Storage.ApplicationData.Current.LocalSettings.Values[SEARCH_WHOLE_WORD] = mWholeword.IsChecked;
            else
                Windows.Storage.ApplicationData.Current.LocalSettings.Values.Add(SEARCH_WHOLE_WORD, mWholeword.IsChecked);
        }

        private void mRectWidth_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            float value;
            if (float.TryParse(mRectWidth.Text, out value))
            {
                if (value < 0.5)
                    value = 0.5f;
                else if (value > 8)
                    value = 8;
                if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(RECT_WIDTH))
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values[RECT_WIDTH] = value;
                else
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values.Add(RECT_WIDTH, value);
                PDFView.rectWidth = value;
            }
        }

        private void mEllilpseWidth_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            float value;
            if (float.TryParse(mEllilpseWidth.Text, out value))
            {
                if (value < 0.5)
                    value = 0.5f;
                else if (value > 8)
                    value = 8;
                if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(ELLIPSE_WIDTH))
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values[ELLIPSE_WIDTH] = value;
                else
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values.Add(ELLIPSE_WIDTH, value);
                PDFView.ovalWidth = value;
            }
        }

        private void OnBackPress(object sender, CancelEventArgs e)
        {
            //settings.Save();
        }

        private void mInkColor_Tap(object sender, TappedRoutedEventArgs e)
        {
            //if (mColorPickerControl == null)
            //{
            mColorPickerControl = new ColorPickerControl(PDFReader.ColorPickerControl.ColorType.TypeInk);
            //}
            mColorPickerControl.OnColorAvailable += OnInkColorAvailable;
            mColorPickerControl.show();
        }

        private void mRectColor_Tap(object sender, TappedRoutedEventArgs e)
        {
            //if (mColorPickerControl == null)
            //{
            mColorPickerControl = new ColorPickerControl(PDFReader.ColorPickerControl.ColorType.TypeRect);
            //}
            mColorPickerControl.OnColorAvailable += OnRectColorAvailable;
            mColorPickerControl.show();
        }

        private void mEllipseColor_Tap(object sender, TappedRoutedEventArgs e)
        {
            //if (mColorPickerControl == null)
            //{
            mColorPickerControl = new ColorPickerControl(PDFReader.ColorPickerControl.ColorType.TypeEllipse);
            //}
            mColorPickerControl.OnColorAvailable += OnEllipseColorAvailable;
            mColorPickerControl.show();
        }

        private void mMarkupColor_Tap(object sender, TappedRoutedEventArgs e)
        {
            Rectangle rect = (Rectangle)sender;
            if (!int.TryParse(rect.Tag.ToString(), out mMarkupType))
                return;
            //if (mColorPickerControl == null)
            //{
            mColorPickerControl = new ColorPickerControl(PDFReader.ColorPickerControl.ColorType.TypeMarkup);
            //}
            mColorPickerControl.OnColorAvailable += OnMarkupColorAvailable;
            mColorPickerControl.show();
        }

        private void OnInkColorAvailable(Color color)
        {
            uint uintColor = 0;
            uintColor = (uint)(color.A << 24 | color.R << 16 | color.G << 8 | color.B);
            mInkColor.Fill = new SolidColorBrush(color);
            if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(INK_COLOR))
                Windows.Storage.ApplicationData.Current.LocalSettings.Values[INK_COLOR] = uintColor;
            else
                Windows.Storage.ApplicationData.Current.LocalSettings.Values.Add(INK_COLOR, uintColor);
            PDFView.inkColor = uintColor;
            mColorPickerControl.dismiss();
        }

        private void OnRectColorAvailable(Color color)
        {
            uint uintColor = 0;
            uintColor = (uint)(color.A << 24 | color.R << 16 | color.G << 8 | color.B);
            mRectColor.Fill = new SolidColorBrush(color);
            if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(RECT_COLOR))
                Windows.Storage.ApplicationData.Current.LocalSettings.Values[RECT_COLOR] = uintColor;
            else
                Windows.Storage.ApplicationData.Current.LocalSettings.Values.Add(RECT_COLOR, uintColor);
            PDFView.rectColor = uintColor;
            mColorPickerControl.dismiss();
        }

        private void OnEllipseColorAvailable(Color color)
        {
            uint uintColor = 0;
            uintColor = (uint)(color.A << 24 | color.R << 16 | color.G << 8 | color.B);
            mEllipseColor.Fill = new SolidColorBrush(color);
            if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(ELLIPSE_COLOR))
                Windows.Storage.ApplicationData.Current.LocalSettings.Values[ELLIPSE_COLOR] = uintColor;
            else
                Windows.Storage.ApplicationData.Current.LocalSettings.Values.Add(ELLIPSE_COLOR, uintColor);
            PDFView.ovalColor = uintColor;
            mColorPickerControl.dismiss();
        }

        private void OnMarkupColorAvailable(Color color)
        {
            uint uintColor = 0;
            uintColor = (uint)(color.A << 24 | color.R << 16 | color.G << 8 | color.B);
            switch (mMarkupType)
            {
                case 0:
                    if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(HIGHLIGHT_COLOR))
                        Windows.Storage.ApplicationData.Current.LocalSettings.Values[HIGHLIGHT_COLOR] = uintColor;
                    else
                        Windows.Storage.ApplicationData.Current.LocalSettings.Values.Add(HIGHLIGHT_COLOR, uintColor);
                    mHighlightColor.Fill = new SolidColorBrush(color);
                    break;
                case 1:
                    if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(UNDER_LINE_COLOR))
                        Windows.Storage.ApplicationData.Current.LocalSettings.Values[UNDER_LINE_COLOR] = uintColor;
                    else
                        Windows.Storage.ApplicationData.Current.LocalSettings.Values.Add(UNDER_LINE_COLOR, uintColor);
                    mUnderlineColor.Fill = new SolidColorBrush(color);
                    break;
                case 2:
                    if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(STRIK_OUT_COLOR))
                        Windows.Storage.ApplicationData.Current.LocalSettings.Values[STRIK_OUT_COLOR] = uintColor;
                    else
                        Windows.Storage.ApplicationData.Current.LocalSettings.Values.Add(STRIK_OUT_COLOR, uintColor);
                    mStrikoutColor.Fill = new SolidColorBrush(color);
                    break;
                case 3:
                    if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(HIGHLIGHT_COLOR_2))
                        Windows.Storage.ApplicationData.Current.LocalSettings.Values[HIGHLIGHT_COLOR_2] = uintColor;
                    else
                        Windows.Storage.ApplicationData.Current.LocalSettings.Values.Add(HIGHLIGHT_COLOR_2, uintColor);
                    mHighlightColor2.Fill = new SolidColorBrush(color);
                    break;
                case 4:
                    if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(SQUIGGLY_UNDER_LINE_COLOR))
                        Windows.Storage.ApplicationData.Current.LocalSettings.Values[SQUIGGLY_UNDER_LINE_COLOR] = uintColor;
                    else
                        Windows.Storage.ApplicationData.Current.LocalSettings.Values.Add(SQUIGGLY_UNDER_LINE_COLOR, uintColor);
                    mSquigglyUnderlineColor.Fill = new SolidColorBrush(color);
                    break;

            }
            mColorPickerControl.dismiss();
            mMarkupType = -1;
        }
    }
}