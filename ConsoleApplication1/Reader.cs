using MasterSlaveConfig;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    public static class IpEndPointReader
    {
        public static IEnumerable<IPEndPoint> GetIpEndPoints()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var sections = (MasterSlaveConfigSection)config.Sections["connectionSettings"];
            ServiceIpEndPointCollection ipEndPoints = sections.ServiceIpEndPointItems;
            for (int i = 0; i < ipEndPoints.Count; i++)
            {
                IPAddress ipAddr = IPAddress.Parse(ipEndPoints[i].Ip);
                yield return new IPEndPoint(ipAddr, ipEndPoints[i].Port);
            }
        }
    }
}
