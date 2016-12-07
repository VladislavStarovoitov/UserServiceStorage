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

        public IEnumerable<User> FindAll(Predicate<User> match)
        {
            return _serviceStorage.FindAll(match);
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

                    //Task.Run(() =>
                    Process(client);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.Message);//добавить логирование
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);//добавить логирование
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
                  
                    ChooseOperation(message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);//добавить логирование
            }
            finally
            {
                if (!ReferenceEquals(client, null))
                {
                    ((TcpClient)client).Close();
                }
            }
        }

        private void ChooseOperation(MasterSlaveMessage<User> message)
        {
            switch (message.Code)
            {
                case MessageCode.Add:
                    _serviceStorage.AddRange(message.Items);
                    Console.WriteLine(_serviceStorage.Find(x => x.Id == message.Items.First().Id).FirstName);
                    break;

                case MessageCode.Remove:
                    _serviceStorage.RemoveAll(i => message.Items.Any(mI => mI.Equals(i)));
                    break;

                case MessageCode.Update:
                    _serviceStorage.UpdateAll(message.Items);
                    break;
            }
        }
    }
}
