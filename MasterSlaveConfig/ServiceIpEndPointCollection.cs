using System.Configuration;

namespace MasterSlaveConfig
{
    [ConfigurationCollection(typeof(ServiceIpEndPoint))]
    public class ServiceIpEndPointCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ServiceIpEndPoint();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ServiceIpEndPoint)element).Port;
        }

        public ServiceIpEndPoint this[int idx] => (ServiceIpEndPoint)BaseGet(idx);
    }
}
