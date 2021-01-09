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
        // save session data for each user that tries to authenticate
        internal static Dictionary<string, UserRequest> usersRequestsDB = new Dictionary<string, UserRequest>();

        internal static Dictionary<string, byte[]> usersDB = new Dictionary<string, byte[]>();
        static SHA256 sha256Hash = SHA256.Create();

        static Database()
        {
            // add client accounts
            usersDB.Add("wcfClient", computeHash("pass"));
            usersDB.Add("wcfClient1", computeHash("pass1"));
            usersDB.Add("wcfClient2", computeHash("pass2"));

            // add service accounts
            usersDB.Add("wcfService", computeHash("svc"));
            usersDB.Add("wcfService1", computeHash("svc1"));

            byte[] computeHash(string password)
            {
                return sha256Hash.ComputeHash(Encoding.ASCII.GetBytes(password));
            }

        }


    }
}
