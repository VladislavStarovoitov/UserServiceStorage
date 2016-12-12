using System;
using System.Collections.Generic;
using System.Linq;
using WcfServiceLibrary.Interfaces;
using MasterSlaveReplication;
using System.Net;
using System.ServiceModel;
using System.IO;

namespace WcfServiceLibrary.Services
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class MasterService : IMasterService
    {
        private static Master master;

        static MasterService()
        {
            IEnumerable<IPEndPoint> ipEndPoints = IpEndPointReader.GetIpEndPoints().ToList();
            var appDomainSetup = new AppDomainSetup
            {
                ApplicationBase = AppDomain.CurrentDomain.BaseDirectory,
                PrivateBinPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Master")
            };
            AppDomain masterDomain = AppDomain.CreateDomain("Master", null, appDomainSetup);

            master =  (Master)masterDomain.CreateInstanceAndUnwrap(typeof(Master).Assembly.FullName,
                typeof(Master).FullName, false, System.Reflection.BindingFlags.Default, null,
                new object[] { ipEndPoints }, null, null);
        }

        public int Add(UserDataContract user)
            => master.Add(user.ToUser());

        public void AddRange(IEnumerable<UserDataContract> collection)
            => master.AddRange(collection.Select(u => u.ToUser()));

        public UserDataContract FindById(int id)
            => master.Find(id).ToUserDataContract();

        public UserDataContract Find(UserDataContract user)
            => master.Find(user.ToUser()).ToUserDataContract();

        public IEnumerable<UserDataContract> FindByFirstName(string firstName)
            => master.FindByFirstName(firstName).Select(u => u.ToUserDataContract());


        public IEnumerable<UserDataContract> FindByLastName(string lastName)
            => master.FindByLastName(lastName).Select(u => u.ToUserDataContract());

        public IEnumerable<UserDataContract> FindByDateOfBirth(DateTime dateOfBirth)
            => master.FindByDateOfBirth(dateOfBirth).Select(u => u.ToUserDataContract());

        public IEnumerable<UserDataContract> GetAll()
            => master.GetAll().Select(u => u.ToUserDataContract()).ToList();

        public void Update(UserDataContract user)
            => master.Update(user.ToUser());

        public void UpdateAll(IEnumerable<UserDataContract> users)
            => master.UpdateAll(users.Select(u => u.ToUser()));

        public void Load()
            => master.Load();

        public void Save()
            => master.Save();

        public bool Remove(UserDataContract user)
        {
            return master.Remove(user.ToUser());
        }

        public bool RemoveById(int id)
        {
            return master.Remove(id);
        }
    }
}
