using MyServiceLibrary;
using System;
using System.Collections.Generic;

namespace MasterSlaveReplication
{
    public interface ISlave
    {
        User Find(User item);

        User Find(int id);

        IEnumerable<User> GetAll();

        IEnumerable<User> FindAll(Predicate<User> match);
    }
}
