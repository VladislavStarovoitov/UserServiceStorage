using MyServiceLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MasterSlaveReplication.Message
{
    [Serializable]
    class MasterSlaveMessage<T>
    {
        public MessageCode Code;
        public IEnumerable<T> Items;
    }
}
