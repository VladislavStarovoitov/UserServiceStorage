using MyServiceLibrary;
using MyServiceLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;

namespace ServiceApplication
{
    public class UserXmlSaver : ISaver<User>
    {
        public IEnumerable<User> Load()
        {
            throw new NotImplementedException();
        }

        public void Save(IEnumerable<User> items)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(User[]));

            using (FileStream fs = new FileStream("people.xml", FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, items.ToArray());
            }
        }
    }
}
