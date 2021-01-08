using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace DomainController.Auditing
{
    public enum AuditEventTypes
    {
        AuthenticationSuccess,
        AuthenticationFailure,
        ValidationSuccess,
        ValidationFailure
    }
    class AuditEvents
    {
        private static ResourceManager resourceManager = null;
        private static object resourceLock = new object();

        private static ResourceManager ResourceManager
        {
            get
            {
                lock (resourceLock)
                {
                    if(resourceManager == null)
                    {
                        resourceManager = new ResourceManager(typeof(AuditEventFile).ToString(), Assembly.GetExecutingAssembly());
                    }

                    return resourceManager;
                }
            }
        }

        public static string AuthenticationSuccess
        {
            get
            {
                return ResourceManager.GetString(AuditEventTypes.AuthenticationSuccess.ToString());
            }
        }

        public static string AuthenticationFailure
        {
            get
            {
                return ResourceManager.GetString(AuditEventTypes.AuthenticationFailure.ToString());
            }
        }

        public static string ValidationSuccess
        {
            get
            {
                return ResourceManager.GetString(AuditEventTypes.ValidationSuccess.ToString());
            }
        }

        public static string ValidationFailure
        {
            get
            {
                return ResourceManager.GetString(AuditEventTypes.ValidationFailure.ToString());
            }
        }
    }
}
