using MyServiceLibrary;
using ServiceApplication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MasterSlaveReplication
{
    public class MasterSlaveConnector
    {
        protected UserServiceStorage _serviceStorage; //заменить на IServiceStorage

        public MasterSlaveConnector(UserServiceStorage serviceStorage = null) //заменить на IServiceStorage
        {
            if (ReferenceEquals(serviceStorage, null))
            {
                serviceStorage = new UserServiceStorage(new UserXmlSaver());
            }
            _serviceStorage = serviceStorage;

            IniteConnection();
        }

        private void IniteConnection()
        {
            
        }
    }
}
