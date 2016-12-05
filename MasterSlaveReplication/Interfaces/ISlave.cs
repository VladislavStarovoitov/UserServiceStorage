using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterSlaveReplication.Interfaces
{
    interface ISlave<T>
    {
        T Find(int id);
        T Find(T item);
        IEnumerable<T> GetAll();
    }
}
