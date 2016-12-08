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

            
            Master master = new Master(a, new UserServiceStorage(new UserXmlSaver()));

            master.Add(new User {LastName = "Star", FirstName = "Vlad", DateOfBirth = DateTime.Now });

            var u = new User { LastName = "Star", FirstName = "Nim", DateOfBirth = DateTime.Now };

            master.Add(u);

            Thread.Sleep(1000);

            var user = master.Find(1);
            user.FirstName = "123";
            master.Update(user);

            Thread.Sleep(1000);
            var f1 = master.Find(2);
            var f2 = slaves[0].Find(u);

            master.Remove(1);
            Thread.Sleep(1000);
        }
    }
}
