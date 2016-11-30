using MasterSlaveReplication.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MasterSlaveReplication
{
    public class Master<T> : MasterSlaveConnector, IMaster<T>
    {
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
