using MasterSlaveReplication.Interfaces;
using MyServiceLibrary;
using ServiceApplication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterSlaveReplication
{
    [Slave]
    public class Slave<T> : MasterSlaveConnector, ISlave<T>
    {
        //заменить на IServiceStorage
        public Slave(UserServiceStorage serviceStorage = null) : base(serviceStorage)
        {
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
    }
}
