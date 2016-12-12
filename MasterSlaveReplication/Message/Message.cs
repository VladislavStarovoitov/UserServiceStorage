using System;
using System.Collections.Generic;

namespace MasterSlaveReplication.Message
{
    [Serializable]
    class MasterSlaveMessage<T>
    {
        public MessageCode Code;
        public IEnumerable<T> Items;
    }
}
