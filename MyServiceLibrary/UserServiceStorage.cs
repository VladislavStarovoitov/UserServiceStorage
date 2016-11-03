using System;
using System.Collections.Generic;
using System.Linq;

namespace MyServiceLibrary
{
    public class UserServiceStorage
    {
        private int _lastId = 0;
        private List<User> _users = new List<User>();
        private IIdGenerator _generator;

        public UserServiceStorage(IIdGenerator generator)
        {
            if (ReferenceEquals(generator, null))
            {
                throw new ArgumentNullException();
            }

            _generator = generator;
        }

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

        public int Add(User user)
        {
            CheckUser(user);

            user.Id = _generator.GenerateId(_lastId);
            _users.Add(user);
            return user.Id;
        }

        public bool Remove(int id)
        {
            CheckId(id);

            var removingUser = _users.FirstOrDefault(u => u.Id == id);
            return RemoveUser(removingUser);
        }

        public bool Remove(User user)
        {
            CheckUser(user);
            var removingUser = _users.FirstOrDefault(u => u.FirstName == user.FirstName && 
                u.LastName == user.LastName && u.DateOfBirth == user.DateOfBirth);
            return RemoveUser(removingUser);
        }

        public int RemoveAll(Predicate<User> match)
        {
            if (ReferenceEquals(match, null))
            {
                throw new ArgumentNullException(nameof(match));
            }

            return _users.RemoveAll(match);
        }

        public User Find(int id)
        {
            CheckId(id);
            return _users.FirstOrDefault(u => u.Id == id);
        }

        public User Find(User user)
        {
            CheckUser(user);
            return _users.FirstOrDefault(u => u.FirstName == user.FirstName &&
                u.LastName == user.LastName && u.DateOfBirth == user.DateOfBirth);
        }

        public List<User> FindAll(Predicate<User> match)
        {
            if (ReferenceEquals(match, null))
            {
                throw new ArgumentNullException(nameof(match));
            }

            List<User> list = new List<User>();
            foreach (var item in _users)
            {
                if (match(item))
                {
                    list.Add(item);
                }
            }

            return list;
        }

        private void CheckUser(User user)
        {
            if (ReferenceEquals(user, null))
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (ReferenceEquals(user.FirstName, null))
            {
                throw new InvalidUserException(nameof(user.FirstName));
            }

            if (ReferenceEquals(user.LastName, null))
            {
                throw new InvalidUserException(nameof(user.LastName));
            }

            if (user.DateOfBirth > DateTime.Now)
            {
                throw new InvalidUserException(nameof(user.DateOfBirth));
            }
        }

        private void CheckId(int id)
        {
            if (id < 0 && id > _lastId)
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        private bool RemoveUser(User removingUser)
        {
            if (ReferenceEquals(removingUser, null))
            {
                return false;
            }

            _users.Remove(removingUser);
            return true;
        }
    }
}
