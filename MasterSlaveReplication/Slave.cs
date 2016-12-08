using MyServiceLibrary;
using ServiceApplication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using MasterSlaveReplication.Message;
using MyServiceLibrary.Interfaces;

namespace MasterSlaveReplication
{
    [Slave]
    public class Slave : MarshalByRefObject
    {
        private IServiceStorage<User> _serviceStorage;
        private ReaderWriterLockSlim _locker = new ReaderWriterLockSlim();

        public Slave(IPEndPoint localEndpoint, IServiceStorage<User> serviceStorage)
        {
            if (ReferenceEquals(serviceStorage, null))
            {
                throw new ArgumentNullException(nameof(serviceStorage));
            }
            _serviceStorage = serviceStorage;

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
                return _serviceStorage.GetAll();
            }
            finally
            {
                _locker.ExitReadLock();
            }
        }

        public IEnumerable<User> FindAll(Predicate<User> match)
        {
            _locker.EnterReadLock();
            try
            {
                return _serviceStorage.FindAll(match);
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
    }
}
