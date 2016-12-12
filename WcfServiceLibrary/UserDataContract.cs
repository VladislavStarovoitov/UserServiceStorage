using System;
using System.Runtime.Serialization;

namespace WcfServiceLibrary
{
    [DataContract]
    public class UserDataContract
    {
        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public DateTime DateOfBirth { get; set; }

        [DataMember]
        public int Id { get; set; }
    }
}
