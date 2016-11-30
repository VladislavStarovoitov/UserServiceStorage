using MyServiceLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace MyServiceLibrary
{
    public class UserServiceStorage : IEnumerable<User>, IServiceStorage<User>
    {
        private int _lastId = 0;
        private List<User> _users = new List<User>();
        private IIdGenerator _generator;
        private ISaver<User> _saver;

        public UserServiceStorage(ISaver<User> saver) : this(saver, new DefaultIdGenerator())
        {
        }

        public UserServiceStorage(ISaver<User> saver, IIdGenerator generator) : this(saver, generator, Enumerable.Empty<User>())
        {
        }

        public UserServiceStorage(ISaver<User> saver, IIdGenerator generator, IEnumerable<User> collection)
        {
            IdGenerator = generator;
            Saver = saver;
            AddRange(collection);
        }

        public IIdGenerator IdGenerator
        {
            set
            {
                if (ReferenceEquals(value, null))
                {
                    throw new ArgumentNullException();
                }

                _generator = value;
            }
        }

        public ISaver<User> Saver
        {
            set
            {
                if (ReferenceEquals(value, null))
                {
                    throw new ArgumentNullException();
                }

                _saver = value;
            }
        }

        public int Add(User user)
        {
            CheckUser(user);

            user.Id = _generator.GenerateId(_lastId);
            _lastId = user.Id;
            _users.Add(user);
            return user.Id;
        }

        public void AddRange(IEnumerable<User> collection)
        {
            if (ReferenceEquals(collection, null))
            {
                throw new ArgumentNullException();
            }

            foreach (var item in collection)
            {
                Add(item);
            }
        }

        public bool Remove(int id)
        {
            CheckId(id);

            var removingUser = _users.FirstOrDefault(u => u.Id == id);
            return RemoveUser(removingUser);
        }

        public bool Remove(Predicate<User> match)
        {
            if (ReferenceEquals(match, null))
            {
                throw new ArgumentNullException(nameof(match));
            }
            var removingUser = _users.FirstOrDefault(u => match(u));
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

        public void Save()
        {
            _saver.Save(_users);
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

        public IEnumerator<User> GetEnumerator()
        {
            return _users.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private class DefaultIdGenerator : IIdGenerator
        {
            public int GenerateId(int lastId) => lastId++;
        }
    }
}