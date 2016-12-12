using MasterSlaveReplication;
using System;
using System.Collections.Generic;
using System.IO;
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
            var appDomainSetup = new AppDomainSetup
            {
                ApplicationBase = AppDomain.CurrentDomain.BaseDirectory,
                PrivateBinPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Slave")
            };

            foreach (var item in ipEndPoints)
            {
                AppDomain slaveDomain = AppDomain.CreateDomain("Slave" + item.Port.ToString(), null, appDomainSetup);

                _slaves.Add((Slave)slaveDomain.CreateInstanceAndUnwrap(typeof(Slave).Assembly.FullName,
                    typeof(Slave).FullName, false, System.Reflection.BindingFlags.Default, null,
                    new object[] { item }, null, null));
            }
        }

        public UserDataContract FindById(int slaveNumber, int id)
        {
            CheckSlaveNumber(slaveNumber);
            return _slaves[slaveNumber].Find(id).ToUserDataContract();
        }

        public UserDataContract Find(int slaveNumber, UserDataContract user)
        {
            CheckSlaveNumber(slaveNumber);
            return _slaves[slaveNumber].Find(user.ToUser()).ToUserDataContract();
        }

        public IEnumerable<UserDataContract> FindByFirstName(int slaveNumber, string firstName)
        {
            CheckSlaveNumber(slaveNumber);
            return _slaves[slaveNumber].FindByFirstName(firstName).Select(u => u.ToUserDataContract());
        }

        public IEnumerable<UserDataContract> FindByLastName(int slaveNumber, string lastName)
        {
            CheckSlaveNumber(slaveNumber);
            return _slaves[slaveNumber].FindByLastName(lastName).Select(u => u.ToUserDataContract());
        }

        public IEnumerable<UserDataContract> FindByDateOfBirth(int slaveNumber, DateTime dateOfBirth)
        {
            CheckSlaveNumber(slaveNumber);
            return _slaves[slaveNumber].FindByDateOfBirth(dateOfBirth).Select(u => u.ToUserDataContract());
        }

        public IEnumerable<UserDataContract> GetAll(int slaveNumber)
        {
            CheckSlaveNumber(slaveNumber);
            return _slaves[slaveNumber].GetAll().Select(u => u.ToUserDataContract());
        }

        private void CheckSlaveNumber(int slaveNumber)
        {
            if (slaveNumber > _slaves.Count - 1)
            {
                throw new ArgumentOutOfRangeException();
            }
        }
    }
}
