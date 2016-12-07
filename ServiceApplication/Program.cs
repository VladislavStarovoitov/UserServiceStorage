using MyServiceLibrary;
using MasterSlaveReplication;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Threading;

namespace ServiceApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            AppDomain masterDomain = AppDomain.CreateDomain("Master");
            AppDomain slaveDomain0 = AppDomain.CreateDomain("Slave0");
            AppDomain slaveDomain1 = AppDomain.CreateDomain("Slave1");

            Slave[] slaves = new Slave[2];
            string ip = "127.0.0.1";

            IPAddress ipAddr = IPAddress.Parse(ip);

            IPEndPoint[] a = new IPEndPoint[] { new IPEndPoint(ipAddr, 5556), new IPEndPoint(ipAddr, 5557) };
            slaves[0] = new Slave(a[0], new UserServiceStorage(new UserXmlSaver()));
            slaves[1] = new Slave(a[1], new UserServiceStorage(new UserXmlSaver())); 

            UserServiceStorage storage = (UserServiceStorage)masterDomain.CreateInstanceAndUnwrap(typeof(UserServiceStorage).Assembly.FullName, typeof(UserServiceStorage).FullName, false, System.Reflection.BindingFlags.Default, null, new object[] { new UserXmlSaver() }, null, null);
            Master master = (Master)masterDomain.CreateInstanceAndUnwrap(typeof(Master).Assembly.FullName, typeof(Master).FullName, false, System.Reflection.BindingFlags.Default, null, new object[] { a, storage }, null, null);

            master.Add(new User {LastName = "Star", FirstName = "Vlad", DateOfBirth = DateTime.Now });
            master.Add(new User { LastName = "Star", FirstName = "Nim", DateOfBirth = DateTime.Now });
        }
    }
}
