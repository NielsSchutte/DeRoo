using System;
using System.Text;
using System.Net;


namespace DeRoo_iOS
{
	public class Login
	{
		String username;
		String password;
		RootObject data;

		public Login(String username, String password)
		{
			this.username = username;
			this.password = password;
		}

		public Boolean isActive()
		{
			try
			{
				using (WebClient client = new WebClient())
				{
					var values = new System.Collections.Specialized.NameValueCollection();
					values.Add("gebruikersnaam", username);
					values.Add("wachtwoord", password);
					byte[] response = client.UploadValues("http://www.amkapp.nl/test/loginApp.php", "POST", values);
					string responseString = Encoding.UTF8.GetString(response);
					data = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObject>(responseString);
				}

				if (data != null)
				{
					User user = new User(data.gebruiker[0].gebruiker_id, data.gebruiker[0].token, data.gebruiker[0].gebruiker_naam, data.gebruiker[0].gebruiker_email);
					DataStorage dataStorage = new DataStorage();
					dataStorage.refresh();
					return true;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
			return false;
		}
	}
}