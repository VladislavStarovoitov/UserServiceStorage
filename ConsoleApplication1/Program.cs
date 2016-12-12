using MasterSlaveReplication;
using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            IEnumerable<IPEndPoint> ipEndPoints = IpEndPointReader.GetIpEndPoints().ToList();

            AppDomain masterDomain = AppDomain.CreateDomain("Master");
            var master = (Master)masterDomain.CreateInstanceAndUnwrap(typeof(Master).Assembly.FullName,
                typeof(Master).FullName, false, System.Reflection.BindingFlags.Default, null,
                new object[] { ipEndPoints }, null, null);

            var a = master.GetAll().ToList();
            var b = master.Add(new MyServiceLibrary.User { FirstName = "", LastName = "", DateOfBirth = DateTime.Now });

            master.Load();
        }
    }
}
