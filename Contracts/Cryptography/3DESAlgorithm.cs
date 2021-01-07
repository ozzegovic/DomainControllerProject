using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Cryptography
{
    public class _3DESAlgorithm
    {
        static CipherMode mode = CipherMode.CBC;

        public static byte[] Encrypt(string plaintext, byte[] secretKey)
        {
            // key needs to be 24 bytes

            if (string.IsNullOrEmpty(plaintext))
            {
                return null;
            }

            var secretkey = secretKey;//initialization
            if (secretKey.Length == 32)
            {
                var array = secretKey;//initialization
                int bytesToEliminate = 8;
                int newLength = array.Length - bytesToEliminate;
                secretkey = new byte[newLength];
                Array.Copy(array, bytesToEliminate, secretkey, 0, newLength);
            }
            byte[] plaintextInput = ASCIIEncoding.ASCII.GetBytes(plaintext);     
            byte[] encryptedOutput = null;

            TripleDESCryptoServiceProvider tripleDesCryptoProvider = new TripleDESCryptoServiceProvider
            {
                // Key = ASCIIEncoding.ASCII.GetBytes(secretKey),
                Key = secretkey,
                Mode = mode,
                Padding = PaddingMode.Zeros,
                
            };

            
   
            tripleDesCryptoProvider.GenerateIV();
            ICryptoTransform tripleDesEncryptTransform = tripleDesCryptoProvider.CreateEncryptor();
            encryptedOutput = tripleDesCryptoProvider.IV.Concat(tripleDesEncryptTransform.TransformFinalBlock(plaintextInput, 0, plaintextInput.Length).ToArray()).ToArray();

            return encryptedOutput;

        }


        public static byte[] Decrypt(byte[] input, byte[] secretKey)
        {
            // key needs to be 24 bytes

            if (input == null || input.Length == 0)
            {
                return null;
            }

            var secretkey = secretKey;//initialization
            if (secretKey.Length == 32)
            {
                var array = secretKey;//initialization
                int bytesToEliminate = 8;
                int newLength = array.Length - bytesToEliminate;
                secretkey = new byte[newLength];
                Array.Copy(array, bytesToEliminate, secretkey, 0, newLength);
            }

            byte[] encryptedInput = input;        
            byte[] decryptedOutput = null;

            TripleDESCryptoServiceProvider tripleDesCryptoProvider = new TripleDESCryptoServiceProvider
            {
                Key =secretkey,
                Mode = mode,
                Padding = PaddingMode.Zeros
                
            };

           
           
            tripleDesCryptoProvider.IV = encryptedInput.Take(tripleDesCryptoProvider.BlockSize / 8).ToArray();                	
            ICryptoTransform tripleDesDecryptTransform = tripleDesCryptoProvider.CreateDecryptor();

            decryptedOutput = tripleDesDecryptTransform.TransformFinalBlock(encryptedInput , 8, encryptedInput.Length-8);

            return decryptedOutput;
        }
    }
}

