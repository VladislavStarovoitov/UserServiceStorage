using MyServiceLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceApplication
{
    class Creator
    {
        private UserServiceStorage master;
        private List<Slave> slaves;

        public UserServiceStorage Master => master;
        public List<Slave> Slaves => slaves;

        public static void CreateService()
        {
        }
    }
}
