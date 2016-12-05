using MasterSlaveReplication.Interfaces;
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
    public class Slave<T> : MarshalByRefObject, ISlave<T>
    {
        private IServiceStorage<T> _serviceStorage;

        public Slave(IPEndPoint localEndpoint, IServiceStorage<T> serviceStorage)
        {
            if (ReferenceEquals(serviceStorage, null))
            {
                throw new ArgumentNullException(nameof(serviceStorage));
            }
            _serviceStorage = serviceStorage;

            Thread listenThread = new Thread(new ParameterizedThreadStart(Listen));
            listenThread.Start(localEndpoint);
        }

        public T Find(T item)
        {
            return _serviceStorage.Find(item);
        }

        public T Find(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetAll()
        {
            return _serviceStorage.GetAll();
        }

        public IEnumerable<T> FindAll(Predicate<T> match)
        {
            throw new NotImplementedException();
        }

        private void Listen(Object localEndpoint)
        {
            TcpListener server = null;
            TcpClient client = null;
            try
            {
                server = new TcpListener((IPEndPoint)localEndpoint);
                server.Start();

                while (true)
                {
                    client = ((TcpListener)server).AcceptTcpClient();

                    Task.Run(() => Process(client));
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
                if (ReferenceEquals(server, null))
                {
                    server.Stop();
                }
                if (ReferenceEquals(client, null))
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
                    MasterSlaveMessage<T> message = (MasterSlaveMessage<T>)formatter.Deserialize(stream);
                    ChooseOperation(message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);//добавить логирование
            }
            finally
            {
                if (ReferenceEquals(client, null))
                {
                    ((TcpClient)client).Close();
                }
            }
        }

        private void ChooseOperation(MasterSlaveMessage<T> message)
        {
            switch (message.Code)
            {
                case MessageCode.Add:
                    _serviceStorage.AddRange(message.Items);
                    break;

                case MessageCode.Delete:
                    _serviceStorage.RemoveAll(i => message.Items.Any(mI => mI.Equals(i)));
                    break;

                //case MessageCode.Update:
                //    _serviceStorage.UpdateAll(message.Items);
                //    break;              
            }
        }
    }
}
