using MasterSlaveReplication.Interfaces;
using MasterSlaveReplication.Message;
using MyServiceLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MasterSlaveReplication
{
    public class Master<T> : MarshalByRefObject, IMaster<T>
    {
        private IServiceStorage<T> _serviceStorage;
        private IEnumerable<IPEndPoint> _endpoints;

        public Master(IEnumerable<IPEndPoint> endpoints, IServiceStorage<T> serviceStorage)
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
            _endpoints = endpoints;

            //Thread listenThread = new Thread(new ParameterizedThreadStart(Listen));
            //listenThread.Start(localEndpoint);
        }

        public int Add(T item)
        {
            int id = _serviceStorage.Add(item);
            //Task.Run(() => SendMessage(new MasterSlaveMessage<T> {Code = MessageCode.Add, Items = _serviceStorage.Find(x => x));
            return id;
        }

        public void AddRange(IEnumerable<T> collection)
        {
            throw new NotImplementedException();
        }

        public T Find(T item)
        {
            throw new NotImplementedException();
        }

        public T Find(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Load()
        {
            throw new NotImplementedException();
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        private void SendMessage(MasterSlaveMessage<T> message)
        {

        } 
    }
}
