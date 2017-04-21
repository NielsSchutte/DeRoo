using System;
using UIKit;
using System.Net;
using System.IO;
using System.Text;
using Foundation;
using CoreGraphics;

namespace LoginBestPractice.iOS
{
	public partial class HandboekViewController : UIViewController
	{
		public HandboekViewController(IntPtr handle) : base(handle)
		{
			var webView = new UIWebView(View.Bounds);
			View.AddSubview(webView);
			string fileName = "handboek.pdf"; // remember case-sensitive
			string localDocUrl = Path.Combine(NSBundle.MainBundle.BundlePath, fileName);
			webView.LoadRequest(new NSUrlRequest(new NSUrl(localDocUrl, false)));
			webView.ScalesPageToFit = true;
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
		}



		public void getPDF()
		{
				var webClient = new WebClient();

				webClient.DownloadStringCompleted += (s, e) =>
				{
					var text = e.Result; // get the downloaded text
					string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
					string localFilename = "handboek.pdf";
					string localPath = Path.Combine(documentsPath, localFilename);
					File.WriteAllText(localPath, text); // writes to local storage
				};

				//locatie handboek
				var url = new Uri("amkapp.nl/test/pages/DeRoo/handboek.pdf");

				//encoding
				webClient.Encoding = Encoding.UTF8;

				//downloadd
				webClient.DownloadStringAsync(url);
			}

		public void openPDF()
		{

		}
	}
	
}