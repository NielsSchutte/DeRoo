using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace PDFReader
{
    public delegate void OnSelectionDialogClosedHandler(bool cancel, int selectedItem);
    public partial class TextSelectionControl : UserControl
    {
        public event OnSelectionDialogClosedHandler OnDialogClose;
        private int mType;
        public TextSelectionControl()
        {
            InitializeComponent();
        }

        public void Show()
        {
            if (!SelectionDialogPopup.IsOpen)
                SelectionDialogPopup.IsOpen = true;
        }

        public void Dismiss()
        {
            if (SelectionDialogPopup.IsOpen)
                SelectionDialogPopup.IsOpen = false;
        }
        bool isShown()
        {
            return SelectionDialogPopup.IsOpen;
        }

        void SelectionPopup_Loaded(Object sender, RoutedEventArgs e)
        {
            SelectionDialogPopup.HorizontalOffset = 50;
            SelectionDialogPopup.VerticalOffset = 50;
            button_ok.IsEnabled = false;
        }

        private void buttonTapped(Object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button.Equals(button_ok) && mType != -1)
                OnDialogClose(false, mType);
            else if (button.Equals(button_cancel))
                OnDialogClose(true, -1);
        }

        private void Radio_Click(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            button_ok.IsEnabled = int.TryParse(radioButton.Tag.ToString(), out mType);
        }
    }
}
