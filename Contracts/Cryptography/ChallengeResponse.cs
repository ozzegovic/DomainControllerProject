using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Cryptography
{
    public class ChallengeResponse
    {
        private SHA256 sha256;

        public ChallengeResponse()
        {
            sha256 = SHA256.Create();
        }

        public byte[] Encrypt(string key, short salt)
        {
            string value = key + salt.ToString();
            byte[] response = sha256.ComputeHash(ASCIIEncoding.ASCII.GetBytes(value));
            return response;
        }
    }
}
