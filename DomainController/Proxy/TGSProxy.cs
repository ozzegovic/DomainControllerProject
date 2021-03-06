﻿using Contracts;
using DomainController.TicketGrantingService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace DomainController.Proxy
{
    public class TGSProxy : ChannelFactory<ITicketGrantingService>, ITicketGrantingService, IDisposable
    {
        ITicketGrantingService factory;

        public TGSProxy(NetTcpBinding binding, string address) : base(binding, address)
        {
            factory = this.CreateChannel();
        }

        // checks if the requested service exists in the dnsTable and returns the full address
        public string GetServiceAddress(string serviceAddress)
        {
            try
            {
                return factory.GetServiceAddress(serviceAddress);
            }
            catch (FaultException<SecurityException> ex)
            {

                throw new FaultException<SecurityException>(new SecurityException(ex.Detail.Message));

            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
        }

        // gets the username of the account that started the service
        public string GetServiceUser(string serviceAddress)
        {
            try
            {
                return factory.GetServiceUser(serviceAddress);
            }
            catch (FaultException<SecurityException> ex)
            {

                throw new FaultException<SecurityException>(new SecurityException(ex.Detail.Message));

            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
        }

        // check if requested service is active
        public bool IsServiceOnline(string serviceName)
        {
            try
            {
                return factory.IsServiceOnline(serviceName);
            }
            catch (FaultException<SecurityException> ex)
            {

                throw new FaultException<SecurityException>(new SecurityException(ex.Detail.Message));

            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
        }

        // after service authentication, add it to the active services list
        // save username that started the service
        public bool ActivateService(string serviceName, string username)
        {
            try
            {
                return factory.ActivateService(serviceName, username);
            }
            catch (FaultException<SecurityException> ex)
            {

                throw new FaultException<SecurityException>(new SecurityException(ex.Detail.Message));

            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
        }

        // could not connect to the service, set it to inactive
        public bool DeactivateService(string serviceName)
        {
            try
            {
                return factory.DeactivateService(serviceName);
            }
            catch (FaultException<SecurityException> ex)
            {

                throw new FaultException<SecurityException>(new SecurityException(ex.Detail.Message));

            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
        }

        //generate session key 
        public string GenerateSessionKey()
        {
            try
            {
                return factory.GenerateSessionKey();
            }
            catch (FaultException<SecurityException> ex)
            {

                throw new FaultException<SecurityException>(new SecurityException(ex.Detail.Message));

            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
        }
    }
}
