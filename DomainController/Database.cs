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
        internal static Dictionary<string, byte[]> usersDB= new Dictionary<string, byte[]>();
        static SHA256 sha256Hash = SHA256.Create();

        static Database()
        {
            string user;
            string pass;
            for (int i = 0; i < 10; i++)
            {
                user = "username" + i;
                pass = "password" + i;

                usersDB.Add(user, computeHash(pass));
            }

            byte[] computeHash(string password)
            { 
                return sha256Hash.ComputeHash(ASCIIEncoding.ASCII.GetBytes(password));
               
            }

        }


    }
}
