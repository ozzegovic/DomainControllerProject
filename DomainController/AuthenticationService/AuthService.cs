using Contracts;
using DomainController.AuthenticationService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace DomainController
{
    public class AuthService : IAuthenticationService
    {
        
        short challenge = 300;

        // checking if user exists in the database
        // if user exists send a challenge
        public short AuthenticateUser(string username)
        {

            if (!Database.usersDB.ContainsKey(username))
                throw new FaultException<SecurityException>(new SecurityException("Authentication Service: Username not found"));
            else
            {
                Console.WriteLine($"Authentication service: {username} found.");
                //challenge = GenerateChallenge();
                Console.WriteLine($"Authentication service: Sending challenge.");

                return challenge;
            }
        }


        // after receiving ''response'' from the client/service
        // encrypt the sent challenge with the stored password hash
        // if the received response and the encryption are the same, user authentication is complete
        public bool CheckPassword(string username, byte[] response)
        {

            byte[] key = Database.usersDB[username];
            byte[] resp = _3DESAlgorithm.Encrypt(challenge.ToString(), key);

            if(Equals(resp, response))
            {
                Console.WriteLine($"Authentication service: {username} authenticated.");
                return true; 
            }
            else
            {
                throw new FaultException<SecurityException>(new SecurityException("Authentication Service: Invalid password"));
            }




        }
        // random 16bit number generator
        private short GenerateChallenge()
        {
            Random r = new Random();
            Console.WriteLine($"Authentication service: generated challenge.");
            return Convert.ToInt16(r.Next(short.MaxValue));
        }

        //Checking if byte arrays are equal
        private bool Equals(byte[] a, byte[] b)

        {

            int x = a.Length ^ b.Length;

            for (int i = 0; i < a.Length && i < b.Length; ++i)

            {

                x |= a[i] ^ b[i];

            }

            return x == 0;

        }
    }
}
