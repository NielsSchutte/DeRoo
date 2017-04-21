using Foundation;
using System;
using UIKit;

namespace LoginBestPractice.iOS
{
    public partial class Formulieren : UIViewController
    {
		public Formulieren (IntPtr handle) : base (handle)
        {
			
        }



		partial void testbutton(UIButton sender)
		{
			testlabel.Text = "Moi";
		}
	}
}