using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyServiceLibrary
{
    public interface IIdGenerator
    {
        int GenerateId(int lastId);
    }
}
