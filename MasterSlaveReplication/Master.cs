using MasterSlaveReplication.Message;
using MyServiceLibrary;
using MyServiceLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MasterSlaveReplication
{
    public class Master: MarshalByRefObject
    {
        public IServiceStorage<User> _serviceStorage;
        private IEnumerable<IPEndPoint> _ipEndPoints;

        public Master(IEnumerable<IPEndPoint> endpoints, IServiceStorage<User> serviceStorage)
        {
            if (ReferenceEquals(serviceStorage, null))
            {
                throw new ArgumentNullException(nameof(serviceStorage));
            }

            if (ReferenceEquals(endpoints, null))
            {
                throw new ArgumentNullException(nameof(endpoints));
            }
            _serviceStorage = serviceStorage;
            _ipEndPoints = endpoints;
        }

        public int Add(User item)
        {
            int id = _serviceStorage.Add(item);
            MasterSlaveMessage<User> message = new MasterSlaveMessage<User> { Code = MessageCode.Add, Items = new List<User> { _serviceStorage.Find(x => x.Id == id) } };
            SendMessages(message);
            return id;
        }

        public void AddRange(IEnumerable<User> collection)
        {
            if (ReferenceEquals(collection, null))
            {
                throw new ArgumentNullException(nameof(collection));
            }

            List<int> ids = new List<int>();
            foreach (var item in collection)
            {
                ids.Add(Add(item));
            }
            MasterSlaveMessage<User> message = new MasterSlaveMessage<User>() { Code = MessageCode.Add, Items = _serviceStorage.FindAll(x => ids.Any(i => i == x.Id)) };
            SendMessages(message);
        }

        public User Find(User item)
        {
            return _serviceStorage.Find(item);
        }

        public User Find(int id)
        {
            return _serviceStorage.Find(x => x.Id == id);
        }

        public IEnumerable<User> GetAll()
        {
            return _serviceStorage.GetAll();
        }

        public void Load()
        {
            _serviceStorage.Load();
        }

        public bool Remove(User item)
        {
            bool result = _serviceStorage.Remove(item);
            MasterSlaveMessage<User> message = new MasterSlaveMessage<User>() { Code = MessageCode.Remove, Items = new List<User> { item } };
            SendMessages(message);
            return result;
        }

        public void Save()
        {
            _serviceStorage.Save();
        }

        private void SendMessages(MasterSlaveMessage<User> message)
        {
            foreach (var ipEndPoint in _ipEndPoints)
            {
                Task.Run(() => SendMessage(message, ipEndPoint));
            }
        }

        private void SendMessage(MasterSlaveMessage<User> message, IPEndPoint endPoint)
        {
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    client.Connect(endPoint);
                    using (NetworkStream stream = client.GetStream())
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        formatter.Serialize(stream, message);
                    }
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
    }
}
