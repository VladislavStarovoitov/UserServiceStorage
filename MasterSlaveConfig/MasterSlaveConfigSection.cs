using System.Configuration;

namespace MasterSlaveConfig
{
    public class MasterSlaveConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("ipEndPoints")]
        public ServiceIpEndPointCollection ServiceIpEndPointItems => (ServiceIpEndPointCollection)(base["ipEndPoints"]);
    }
}
