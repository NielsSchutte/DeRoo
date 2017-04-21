// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace LoginBestPractice.iOS
{
    [Register ("LogoutViewController")]
    partial class LogoutViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton LogOutButtonYes { get; set; }

        [Action ("LogOutButtonYes_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void LogOutButtonYes_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (LogOutButtonYes != null) {
                LogOutButtonYes.Dispose ();
                LogOutButtonYes = null;
            }
        }
    }
}