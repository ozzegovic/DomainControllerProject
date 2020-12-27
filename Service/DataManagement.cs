using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class DataManagement : IDataManagement, IDataManagementDC
    {
        // TO DO: Change return type to byte[]
        // decrypt request
        // read data from database
        // encrypt read values and send back to the client
        public bool Read(byte[] encryptedData)
        {
            Console.WriteLine("Received encrypted READ request");
            return true;
        }

        // TO DO: Change return type to byte[]
        // decrypt request
        // write data to the database
        // encrypt data and send back to the client
        public bool Write(byte[] encryptedData)
        {
            Console.WriteLine("Received encrypted WRITE request");
            return true;
        }


        // Domain Controller sends session key after a client requested it
        // Domain controller can remove it from the active services list if it cannot connect or doesnt receive any answer
        // TO DO: 
        public bool SendSessionKey(byte[] sessionKey)
        {
            throw new NotImplementedException();
        }
    }
}
