using MyServiceLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace MyServiceLibrary
{
    public class UserXmlSaver : ISaver<User>
    {
        private string path = "people.xml";

        public IEnumerable<User> Load()
        {
            XmlSerializer formatter = new XmlSerializer(typeof(User[]));
            IEnumerable<User> users = new List<User>();
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    users = (User[])formatter.Deserialize(fs);
                }
            }
            return users;
        }

        public void Save(IEnumerable<User> items)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(User[]));

            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, items.ToArray());
            }
        }
    }
}
