using MasterSlaveReplication.Message;
using MyServiceLibrary;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Linq;

namespace MasterSlaveReplication
{
    public class Master : MarshalByRefObject, ISlave
    {
        private UserServiceStorage _serviceStorage;
        private IEnumerable<IPEndPoint> _ipEndPoints;

        public Master(IEnumerable<IPEndPoint> endpoints)
        {
            if (ReferenceEquals(endpoints, null))
            {
                throw new ArgumentNullException(nameof(endpoints));
            }

            _serviceStorage = new UserServiceStorage(new UserXmlSaver());
            _ipEndPoints = endpoints;
        }

        public int Add(User item)
        {
            _serviceStorage.Add(item);
            SendMessages(MessageCode.Add, new List<User> { item });
            return item.Id;
        }

        public void AddRange(IEnumerable<User> collection)
        {
            if (ReferenceEquals(collection, null))
            {
                throw new ArgumentNullException(nameof(collection));
            }

            _serviceStorage.AddRange(collection);
            SendMessages(MessageCode.Add, collection);
        }

        public User Find(User item)
        {
            return _serviceStorage.Find(item);
        }

        public User Find(int id)
        {
            return _serviceStorage.Find(x => x.Id == id);
        }

        public IEnumerable<User> FindByFirstName(string firstName)
        {
            return _serviceStorage.FindAll(x => x.FirstName == firstName).ToList();
        }

        public IEnumerable<User> FindByLastName(string lastName)
        {
            return _serviceStorage.FindAll(x => x.LastName == lastName).ToList();
        }

        public IEnumerable<User> FindByDateOfBirth(DateTime dateOfBirth)
        {
            return _serviceStorage.FindAll(x => x.DateOfBirth == dateOfBirth).ToList();
        }

    public IEnumerable<User> GetAll()
        {
            return _serviceStorage.GetAll().ToList().ToList();
        }

        public void Update(User user)
        {
            _serviceStorage.Update(user); MasterSlaveMessage<User> message = new MasterSlaveMessage<User> { Code = MessageCode.Update, Items = new List<User> { user } };
            SendMessages(MessageCode.Update, new List<User> { user });
        }

        public void UpdateAll(IEnumerable<User> users)
        {
            _serviceStorage.UpdateAll(users); MasterSlaveMessage<User> message = new MasterSlaveMessage<User> { Code = MessageCode.Update, Items = users };
            SendMessages(MessageCode.Update, users);
        }

        public void Load()
        {
            _serviceStorage.Load();
            SendMessages(MessageCode.Load, _serviceStorage.GetAll().ToList());
        }

        public bool Remove(User item)
        {
            bool result = _serviceStorage.Remove(item);
            SendMessages(MessageCode.Remove, new List<User> { item });
            return result;
        }

        public bool Remove(int id)
        {
            User removingUser = Find(id);
            bool result = _serviceStorage.Remove(x => x.Id == id);
            SendMessages(MessageCode.Remove, new List<User> { removingUser });
            return result;
        }

        public void Save()
        {
            _serviceStorage.Save();
        }

        private void SendMessages(MessageCode code, IEnumerable<User> users)
        {
            MasterSlaveMessage<User> message = new MasterSlaveMessage<User>() { Code = code, Items = users };
            foreach (var ipEndPoint in _ipEndPoints)
            {
                SendMessage(message, ipEndPoint);
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
