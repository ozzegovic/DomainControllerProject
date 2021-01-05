using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    // Only the classes that inherit this one have access to it's fields
    public abstract class ServiceData
    {
        // static so every class has the same instance of the serviceSecret
        protected static byte[] serviceSecret;
    }
}
