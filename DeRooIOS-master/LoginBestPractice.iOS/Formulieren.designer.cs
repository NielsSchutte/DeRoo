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
    [Register ("Formulieren")]
    partial class Formulieren
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel testlabel { get; set; }

        [Action ("testbutton:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void testbutton (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (testlabel != null) {
                testlabel.Dispose ();
                testlabel = null;
            }
        }
    }
}