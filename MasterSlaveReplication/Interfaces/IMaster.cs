using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterSlaveReplication.Interfaces
{
    interface IMaster<T> : ISlave<T>
    {
        int Add(T item);
        void AddRange(IEnumerable<T> collection);
        bool Remove(T item);
        void Save();
        void Load();
    }
}
