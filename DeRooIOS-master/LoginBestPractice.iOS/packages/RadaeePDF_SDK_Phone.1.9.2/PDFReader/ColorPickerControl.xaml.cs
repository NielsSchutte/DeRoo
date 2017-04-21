﻿using RDPDFLib.view;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace PDFReader
{
    public delegate void OnColorAvailableHandler(Color color);
    public partial class ColorPickerControl : UserControl
    {
        public enum ColorType
        {
            TypeInk,
            TypeRect,
            TypeEllipse,
            TypeMarkup
        }

        public event OnColorAvailableHandler OnColorAvailable;

        private ColorType mType;

        private byte red;
        private byte green;
        private byte blue;
        private byte transparency;

        private uint mOriginalColor;
        private uint mPickedColor;

        private bool mInitialized = false;
        public ColorPickerControl(ColorType type)
        {
            this.InitializeComponent();
            mType = type;
            mOriginalColor = 0;
            switch (mType)
            {
                case ColorType.TypeInk:
                    mTitleLabel.Text = "Select doodle line color:";
                    mOriginalColor = PDFView.inkColor;
                    break;
                case ColorType.TypeRect:
                    mTitleLabel.Text = "Select rect line color:";
                    mOriginalColor = PDFView.rectColor;
                    break;
                case ColorType.TypeEllipse:
                    mTitleLabel.Text = "Select ellipse line color:";
                    mOriginalColor = PDFView.ovalColor;
                    break;
                case ColorType.TypeMarkup:
                    mTitleLabel.Text = "Select markup line color:";
                    mOriginalColor = PDFSettingsPage.DefaultHighlightColor;
                    break;
            }


            uint rgbColor = mOriginalColor & 0xff000000;
            transparency = Convert.ToByte(rgbColor >> 24);
            rgbColor = mOriginalColor & 0x00ff0000;
            red = Convert.ToByte(rgbColor >> 16);
            rgbColor = mOriginalColor & 0xff00;
            green = Convert.ToByte(rgbColor >> 8);
            rgbColor = mOriginalColor & 0xff;
            blue = Convert.ToByte(rgbColor);

            ASlider.Value = transparency;
            RSlider.Value = red;
            GSlider.Value = green;
            BSlider.Value = blue;

            Color solidColor = new Color();
            solidColor = Color.FromArgb(0xff, red, green, blue);

            fillView(true, solidColor);
            fillView(false, solidColor);

            mInitialized = true;
        }

        void fillView(bool isOrigin, Color color)
        {

            if (isOrigin)
                mOriginalColorView.Fill = new SolidColorBrush(color);
            else
                mPickColorView.Fill = new SolidColorBrush(color);
        }

        private void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (!mInitialized)
                return;
            transparency = Convert.ToByte(ASlider.Value);
            red = Convert.ToByte(RSlider.Value);
            green = Convert.ToByte(GSlider.Value);
            blue = Convert.ToByte(BSlider.Value);

            Color color = new Color();
            color = Color.FromArgb(transparency, red, green, blue);

            fillView(false, color);
        }

        private void onRestoreBtnClick(object sender, RoutedEventArgs e)
        {
            uint rgbColor = mOriginalColor & 0x00ff0000;
            red = Convert.ToByte(rgbColor >> 16);
            rgbColor = mOriginalColor & 0xff00;
            green = Convert.ToByte(rgbColor >> 8);
            rgbColor = mOriginalColor & 0xff;
            blue = Convert.ToByte(rgbColor);

            Color color = new Color();
            color = Color.FromArgb(0xff, red, green, blue);
            fillView(false, color);
        }

        private void onConfirmBtnClick(object sender, RoutedEventArgs e)
        {
            //mPickedColor = (uint)((transparency << 24) | (red << 16) | (green << 8) | blue);
            //switch (mType)
            //{
            //    case ColorType.TypeInk:
            //        if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey("Ink_color"))
            //            Windows.Storage.ApplicationData.Current.LocalSettings.Values["Ink_color"] = mPickedColor;
            //        else
            //            Windows.Storage.ApplicationData.Current.LocalSettings.Values.Add("Ink_color", mPickedColor);

            //        PDFView.inkColor = mPickedColor;
            //        break;
            //    case ColorType.TypeRect:
            //        if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey("Rect_color"))
            //            Windows.Storage.ApplicationData.Current.LocalSettings.Values["Rect_color"] = mPickedColor;
            //        else
            //            Windows.Storage.ApplicationData.Current.LocalSettings.Values.Add("Rect_color", mPickedColor);

            //        PDFView.rectColor = mPickedColor;
            //        break;
            //    case ColorType.TypeEllipse:
            //        if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey("Ellipse_color"))
            //            Windows.Storage.ApplicationData.Current.LocalSettings.Values["Ellipse_color"] = mPickedColor;
            //        else
            //            Windows.Storage.ApplicationData.Current.LocalSettings.Values.Add("Ellipse_color", mPickedColor);

            //        PDFView.ovalColor = mPickedColor;
            //        break;
            //    case ColorType.TypeMarkup:

            //        break;
            //}
            Color color = new Color();
            transparency = Convert.ToByte(ASlider.Value);
            red = Convert.ToByte(RSlider.Value);
            green = Convert.ToByte(GSlider.Value);
            blue = Convert.ToByte(BSlider.Value);
            color = Color.FromArgb(transparency, red, green, blue);
            OnColorAvailable(color);
            //dismiss();
        }

        private void ColorPickerPopup_Loaded(Object sender, RoutedEventArgs e)
        {
            Windows.UI.Core.CoreWindow rcWindow = Windows.UI.Xaml.Window.Current.CoreWindow;
            Rect rcScreen = rcWindow.Bounds;
            ColorPickerPopup.HorizontalOffset = rcScreen.Width / 2 - 250;
            ColorPickerPopup.VerticalOffset = rcScreen.Height / 2 - 300;
        }

        public void show()
        {
            if (!ColorPickerPopup.IsOpen)
            {
                ColorPickerPopup.IsOpen = true;
            }
        }

        public void dismiss()
        {
            if (ColorPickerPopup.IsOpen)
            {
                ColorPickerPopup.IsOpen = false;
            }
        }

        public bool isShown()
        {
            return ColorPickerPopup.IsOpen;
        }
    }
}
