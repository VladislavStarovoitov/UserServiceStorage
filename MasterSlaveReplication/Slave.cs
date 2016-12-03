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

namespace MasterSlaveReplication
{
    [Slave]
    public class Slave<T> : MasterSlaveConnector, ISlave<T>
    { 
        //заменить на IServiceStorage
        public Slave(IPEndPoint localEndpoint, UserServiceStorage serviceStorage = null) : base(serviceStorage)
        {
            Thread listenThread = new Thread(new ParameterizedThreadStart(Listen));
            listenThread.Start(localEndpoint);
        }

        public T Find(T item)
        {
            throw new NotImplementedException();
        }

        public T Find(int id)
        {
            throw new NotImplementedException();
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
    }
}
