using MyServiceLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using MasterSlaveReplication.Message;

namespace MasterSlaveReplication
{
    public class Slave : MarshalByRefObject, ISlave
    {
        private UserServiceStorage _serviceStorage;
        private ReaderWriterLockSlim _locker = new ReaderWriterLockSlim();

        public Slave(IPEndPoint localEndpoint)
        {
            if (ReferenceEquals(localEndpoint, null))
            {
                throw new ArgumentNullException(nameof(localEndpoint));
            }
            _serviceStorage = new UserServiceStorage(new UserXmlSaver());

            Thread listenThread = new Thread(new ParameterizedThreadStart(Listen));
            listenThread.Start(localEndpoint);
        }

        public User Find(User item)
        {
            _locker.EnterReadLock();
            try
            {
                return _serviceStorage.Find(item);
            }
            finally
            {
                _locker.ExitReadLock();
            }
        }

        public User Find(int id)
        {
            _locker.EnterReadLock();
            try
            {
                return _serviceStorage.Find(x => x.Id == id);
            }
            finally
            {
                _locker.ExitReadLock();
            }
        }

        public IEnumerable<User> GetAll()
        {
            _locker.EnterReadLock();
            try
            {
                return _serviceStorage.GetAll().ToList();
            }
            finally
            {
                _locker.ExitReadLock();
            }
        }


        public IEnumerable<User> FindByFirstName(string firstName)
        {
            _locker.EnterReadLock();
            try
            {
                return _serviceStorage.FindAll(x => x.FirstName == firstName).ToList();
            }
            finally
            {
                _locker.ExitReadLock();
            }
        }

        public IEnumerable<User> FindByLastName(string lastName)
        {
            _locker.EnterReadLock();
            try
            {
                return _serviceStorage.FindAll(x => x.LastName == lastName).ToList();
            }
            finally
            {
                _locker.ExitReadLock();
            }
        }

        public IEnumerable<User> FindByDateOfBirth(DateTime dateOfBirth)
        {
            _locker.EnterReadLock();
            try
            {
                return _serviceStorage.FindAll(x => x.DateOfBirth == dateOfBirth).ToList();
            }
            finally
            {
                _locker.ExitReadLock();
            }
        }

        private void Listen(object localEndpoint)
        {
            TcpListener server = null;
            TcpClient client = null;
            try
            {
                server = new TcpListener((IPEndPoint)localEndpoint);
                server.Start();

                while (true)
                {
                    client = server.AcceptTcpClient();
                    Process(client);
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
            finally
            {
                if (!ReferenceEquals(server, null))
                {
                    server.Stop();
                }
                if (!ReferenceEquals(client, null))
                {
                    client.Close();
                }
            }
        }

        private void Process(object client)
        {
            try
            {
                using (NetworkStream stream = ((TcpClient)client).GetStream())
                {                   
                    BinaryFormatter formatter = new BinaryFormatter();
                    MasterSlaveMessage<User> message = ((MasterSlaveMessage<User>)formatter.Deserialize(stream));
                  
                    HandleRequest(message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (!ReferenceEquals(client, null))
                {
                    ((TcpClient)client).Close();
                }
            }
        }

        private void HandleRequest(MasterSlaveMessage<User> message)
        {
            switch (message.Code)
            {
                case MessageCode.Add:
                    AddRange(message.Items);
                    break;

                case MessageCode.Remove:
                    RemoveAll(message.Items);
                    break;

                case MessageCode.Update:
                    UpdateAll(message.Items);
                    break;

                case MessageCode.Load:
                    Load(message.Items);
                    break;
            }
        }

        private void AddRange(IEnumerable<User> users)
        {
            List<int> ids = users.Select(u => u.Id).ToList();
            _locker.EnterWriteLock();
            try
            {
                _serviceStorage.AddRange(users);
                int i = 0;
                foreach (var item in users)
                {
                    item.Id = ids[i];
                    i++;
                }
            }
            finally
            {
                _locker.ExitWriteLock();
            }
        }

        private void RemoveAll(IEnumerable<User> users)
        {
            _locker.EnterWriteLock();
            try
            {
                _serviceStorage.RemoveAll(i => users.Any(mI => mI.Equals(i)));
            }
            finally
            {
                _locker.ExitWriteLock();
            }
        }

        private void UpdateAll(IEnumerable<User> users)
        {
            _locker.EnterWriteLock();
            try
            {
                _serviceStorage.UpdateAll(users);
            }
            finally
            {
                _locker.ExitWriteLock();
            }
        }

        private void Load(IEnumerable<User> users)
        {
            _locker.EnterWriteLock();
            try
            {
                _serviceStorage.Clear();
                _serviceStorage.AddRange(users);
            }
            finally
            {
                _locker.ExitWriteLock();
            }
        }
    }
}
