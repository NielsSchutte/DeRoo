using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace DeRoo_iOS
{
    class User
    {
        public string id { get; set; }
        public string token { get; set; }
        public string name { get; set; }
        public string email { get; set; }

        public static User instance = null;

        public User(string id, string token, string name, string email)
        {
            this.id = id;
            this.token = token;
            this.name = name;
            this.email = email;

            if (instance == null)
            {
                instance = this;
            }
        }
    }
}