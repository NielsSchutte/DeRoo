using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;

namespace PDFReader
{
    public delegate void OnPasswordAvailableHandler(String password);
    public partial class PasswordControl : UserControl
    {
        public event OnPasswordAvailableHandler OnPasswordAvailable;
        public PasswordControl()
        {
            InitializeComponent();
        }

        public void Show()
        {
            PasswordPopup.IsOpen = true;
        }

        public void Dismiss()
        {
            PasswordPopup.IsOpen = false;
        }

        public void ShowHint()
        {
            password_hint.Visibility = Visibility.Visible;
            password_box.Password = string.Empty;
        }

        private void buttonTapped(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button.Equals(button_cancel))
            {
                OnPasswordAvailable(null);
            }
            else if (button.Equals(button_ok))
            {
                OnPasswordAvailable(password_box.Password);
            }
        }

        void PasswordPopup_Loaded(Object sender, RoutedEventArgs e)
        {
            PasswordPopup.HorizontalOffset = 50;
            PasswordPopup.VerticalOffset = 50;
        }
    }
}
