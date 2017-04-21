using Foundation;
using System;
using UIKit;

namespace LoginBestPractice.iOS
{
    public partial class LogoutViewController : UIViewController
    {

        public LogoutViewController (IntPtr handle) : base (handle)
        {
			
        }

        //Implement a Logout Feature
        partial void LogOutButtonYes_TouchUpInside(UIButton sender)
        {
			//Create an instance of our AppDelegate
			var appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;

			//Get an instance of our MainStoryboard.storyboard
			var mainStoryboard = appDelegate.MainStoryboard;

			//Get an instance of our Login Page View Controller
			var loginPageViewController = appDelegate.GetViewController(mainStoryboard, "LoginPageViewController") as LoginPageViewController;

			//Wire our event handler to show the MainTabBarController after we successfully logged in.
			loginPageViewController.OnLoginSuccess += (s, e) =>
			{
				var tabBarController = appDelegate.GetViewController(mainStoryboard, "MainTabBarController");
				appDelegate.SetRootViewController(tabBarController, true);
			};

			//Set the Login Page as our RootViewController
			appDelegate.SetRootViewController(loginPageViewController, true);
        }
    }
}