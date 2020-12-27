using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public class User
    {
        string username = string.Empty;
        string password = string.Empty;

        public User(string _username, string _password)
        {
            this.Username = _username;
            this.Password = _password;
        }

        public string Password { get => password; set => password = value; }
        public string Username { get => username; set => username = value; }

    }
}

