using System;
using System.Collections.Generic;

namespace MyServiceLibrary
{
    public class UserServiceStorage
    {
        private int lastId = 0;
        private List<User> _users = new List<User>();
        private IIdGenerator _generator;

        public IIdGenerator IdGenerator
        {
            get
            {
                return _generator;
            }

            set
            {
                if (ReferenceEquals(value, null))
                {
                    throw new ArgumentNullException();
                }

                _generator = value;
            }
        }

        public UserServiceStorage(IIdGenerator generator)
        {
            _generator = generator;
        }

        public int Add(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException();
            }

            if (user.FirstName == null)
            {
                throw new InvalidUserException();
            }

            if (user.LastName == null)
            {
                throw new InvalidUserException();
            }

            return 0;
        }
    }
}
