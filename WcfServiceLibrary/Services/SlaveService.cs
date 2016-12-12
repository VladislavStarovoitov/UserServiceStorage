using MasterSlaveReplication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using WcfServiceLibrary.Interfaces;

namespace WcfServiceLibrary.Services
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class SlaveService : ISlaveService
    {
        private static List<Slave> _slaves = new List<Slave>();

        static SlaveService()
        {
            IEnumerable<IPEndPoint> ipEndPoints = IpEndPointReader.GetIpEndPoints().ToList();

            foreach (var item in ipEndPoints)
            {
                AppDomain slaveDomain = AppDomain.CreateDomain("Slave" + item.Port.ToString());

                _slaves.Add((Slave)slaveDomain.CreateInstanceAndUnwrap(typeof(Slave).Assembly.FullName,
                    typeof(Slave).FullName, false, System.Reflection.BindingFlags.Default, null,
                    new object[] { item }, null, null));
            }
        }

        public UserDataContract FindById(int slaveNumber, int id) 
            => _slaves[slaveNumber].Find(id).ToUserDataContract();

        public UserDataContract Find(int slaveNumber, UserDataContract user)
            => _slaves[slaveNumber].Find(user.ToUser()).ToUserDataContract();

        public IEnumerable<UserDataContract> FindByFirstName(int slaveNumber, UserDataContract user)
            => _slaves[slaveNumber].FindAll(u => u.FirstName == user.FirstName).Select(u => u.ToUserDataContract());

        public IEnumerable<UserDataContract> FindByLastName(int slaveNumber, UserDataContract user)
            => _slaves[slaveNumber].FindAll(u => u.LastName == user.LastName).Select(u => u.ToUserDataContract());

        public IEnumerable<UserDataContract> GetAll(int slaveNumber)
            => _slaves[slaveNumber].GetAll().Select(u => u.ToUserDataContract());
    }
}
