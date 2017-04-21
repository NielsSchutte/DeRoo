using System;
using UIKit;
using DeRoo_iOS;
using System.Drawing;

namespace LoginBestPractice.iOS
{
	public partial class FormulierenViewController : UIViewController
	{

		partial void button_TouchUpInside(UIButton sender)
		{
			RootObject data = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObject>(DataStorage.forms);
			int w = 200;

			for (int i = 0; i<data.formulieren.Count; i++)
            {
				w += -40;
				UIButton btn = new UIButton(UIButtonType.RoundedRect);
				btn.SetTitle (data.formulieren[i].formulier_naam, UIControlState.Normal);
				btn.Frame = new RectangleF(50, w, 200, 100);
				btn.TouchDown += delegate 
				{    					UIAlertView alert = new UIAlertView("Hello", "Hello, Xamarin!", null, "OK");
					alert.Show();
				};
				this.View.AddSubview (btn);
				//button.SetTitle (data.formulieren[i].formulier_naam, UIControlState.Normal);
            }
		}

		public FormulierenViewController(IntPtr handle) : base(handle)
		{
			
		}


            
	}
}