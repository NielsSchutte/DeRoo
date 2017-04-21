using System;
using DeRoo_iOS;
using UIKit;

namespace LoginBestPractice.iOS
{
	partial class LoginPageViewController : UIViewController
	{
        //Create an event when a authentication is successful
        public event EventHandler OnLoginSuccess;

		public LoginPageViewController (IntPtr handle) : base (handle)
		{
        }

        partial void LoginButton_TouchUpInside(UIButton sender)
        {
            //Validate our Username &Password.
			//This is usually a web service call.
			if (IsUserNameValid() && IsPasswordValid())
			{

				String tempUsername = UserNameTextView.Text;
				String tempPassword = PasswordTextView.Text;

				Login login = new Login(tempUsername, tempPassword);

				if (login.isActive())
				{
					//redirect
					OnLoginSuccess(sender, new EventArgs());
				}
				else
				{
					UIAlertView alert = new UIAlertView()
					{
						Title = "Warning",
						Message = "Foute gebruikersnaam of wachtwoord!"
					};
					alert.AddButton("OK");
					alert.Show();
				}
			}
			else
			{
				new UIAlertView("Login Error", "Foute gebruikersnaam of wachtwoord!", null, "OK", null).Show();
			}
        }

        private bool IsUserNameValid()
        {
            return !String.IsNullOrEmpty(UserNameTextView.Text.Trim());
        }

        private bool IsPasswordValid()
        {
            return !String.IsNullOrEmpty(PasswordTextView.Text.Trim());
        }
	}
}
