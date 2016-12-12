using System.Configuration;

namespace MasterSlaveConfig
{
    public class ServiceIpEndPoint : ConfigurationElement
    {
        [ConfigurationProperty("port", DefaultValue = 0, IsKey = true, IsRequired = true)]
        public int Port
        {
            get { return (int)base["port"]; }
            set { base["port"] = value; }
        }

        [ConfigurationProperty("ip", DefaultValue = "127.0.0.1", IsKey = false, IsRequired = true)]
        public string Ip
        {
            get { return (string)base["ip"]; }
            set { base["ip"] = value; }
        }
    }
}
