﻿using Contracts.Models;
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
        internal static Dictionary<string, User> usersDB= new Dictionary<string, User>();
        static SHA256 sha256Hash = SHA256.Create();

        static Database()
        {
            string username;
            string pass;
            User user;

            for (int i = 0; i < 10; i++)
            {
                username = "username" + i;
                pass = "password" + i;
                user = new User(username, computeHash(pass).ToString());

                usersDB.Add(username, user);
            }

            byte[] computeHash(string password)
            {
                return sha256Hash.ComputeHash(ASCIIEncoding.ASCII.GetBytes(password));
            }

        }


    }
}
