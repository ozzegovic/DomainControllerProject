using Contracts;
using Contracts.Cryptography;
using Contracts.Models;
using DomainController.AuthenticationService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using DomainController.Auditing;

namespace DomainController
{
    public class AuthService : IAuthenticationService
    {
        // checking if user exists in the database
        // if user exists send a challenge
        public short AuthenticateUser(string username)
        {
            short challenge;

            if (!Database.usersDB.ContainsKey(username))
            {
                throw new FaultException<SecurityException>(new SecurityException($"Authentication Service: Username '{username}' not found"));
            }
            else
            {
                Console.WriteLine($"Authentication service: {username} found.");
                challenge = GenerateChallenge();
                Console.WriteLine($"Authentication service: Sending challenge.");
                return challenge;
            }
        }


        // after receiving ''response'' from the client/service
        // encrypt the sent challenge with the stored password hash
        // if the received response and the encryption are the same, user authentication is complete
        // Logs result
        public bool CheckPassword(UserRequest userRequest, byte[] response)
        {
            byte[] passHash;
            if(!Database.usersDB.TryGetValue(userRequest.Username, out passHash))
            {
                throw new FaultException<SecurityException>(new SecurityException($"Authentication Service: Username '{userRequest.Username}' doesn't exist"));
            }

            ChallengeResponse cr = new ChallengeResponse();
            byte[] expected = cr.Encrypt(passHash, userRequest.Challenge);

            if(Equals(expected, response))
            {
                try
                {
                    Audit.AuthenticationSuccess(userRequest.Username);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                Console.WriteLine($"Authentication service: {userRequest.Username} authenticated.");
                return true; 
            }
            else
            {
                try
                {
                    Audit.AuthenticationFailure(userRequest.Username);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                throw new FaultException<SecurityException>(new SecurityException("Authentication Service: Invalid password"));
            }
        }

        // random 16bit number generator
        private short GenerateChallenge()
        {
            Random r = new Random();
            Console.WriteLine("Authentication service: generated challenge.");
            return Convert.ToInt16(r.Next(0, short.MaxValue));
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
