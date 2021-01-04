using Contracts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DomainController
{
    public class Database
    {
        // save session data
        internal static Dictionary<string, UserRequest> usersRequestsDB = new Dictionary<string, UserRequest>();

        internal static Dictionary<string, byte[]> usersDB = new Dictionary<string, byte[]>();
        static SHA256 sha256Hash = SHA256.Create();

        static Database()
        {
            string username;
            string pass;

            for (int i = 0; i < 10; i++)
            {
                username = "username" + i;
                pass = "password" + i;

                usersDB.Add(username, computeHash(pass));
            }

            // add service accounts
            usersDB.Add("DataManagementService", computeHash("pass"));

            byte[] computeHash(string password)
            {
                return sha256Hash.ComputeHash(Encoding.ASCII.GetBytes(password));
            }

        }


    }
}
