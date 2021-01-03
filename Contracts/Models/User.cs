using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Models
{
    public class User
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public short LastChallenge { get; set; }

        public User(string username, string passwordHash)
        {
            Username = username;
            PasswordHash = passwordHash;
            LastChallenge = -1;
        }
    }
}

