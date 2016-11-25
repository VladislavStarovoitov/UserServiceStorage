using System;

namespace MyServiceLibrary
{
    [Serializable]
    public class User
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public int Id { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return false;
            }

            User otherUser = obj as User;
            if (ReferenceEquals(otherUser, null))
            {
                return false;
            }

            if (otherUser.Id == Id)
            {
                return true;
            }

            return otherUser.FirstName == FirstName && otherUser.LastName == LastName &&
                otherUser.DateOfBirth == DateOfBirth;
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}
