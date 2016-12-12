using System;
using System.Collections.Generic;
using System.Linq;
using WcfServiceLibrary.Interfaces;
using MasterSlaveReplication;
using System.Net;
using System.ServiceModel;

namespace WcfServiceLibrary.Services
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class MasterService : IMasterService
    {
        private static Master master;

        static MasterService()
        {
            IEnumerable<IPEndPoint> ipEndPoints = IpEndPointReader.GetIpEndPoints();

            AppDomain masterDomain = AppDomain.CreateDomain("Master");
            master = (Master)masterDomain.CreateInstanceAndUnwrap(typeof(Master).Assembly.FullName,
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

        public IEnumerable<UserDataContract> FindByFirstName(UserDataContract user)
            => master.FindAll(u => u.FirstName == user.FirstName).Select(u => u.ToUserDataContract());


        public IEnumerable<UserDataContract> FindByLastName(UserDataContract user)
            => master.FindAll(u => u.LastName == user.LastName).Select(u => u.ToUserDataContract());


        public IEnumerable<UserDataContract> GetAll()
            => master.GetAll().Select(u => u.ToUserDataContract());

        public void Update(UserDataContract user)
            => master.Update(user.ToUser());

        public void UpdateAll(IEnumerable<UserDataContract> users)
            => master.UpdateAll(users.Select(u => u.ToUser()));

        public void Load()
            => master.Load();

        public void Save()
            => master.Save();
    }
}
