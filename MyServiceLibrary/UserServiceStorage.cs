using MyServiceLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace MyServiceLibrary
{
    public class UserServiceStorage : MarshalByRefObject, IEnumerable<User>, IServiceStorage<User>
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

            _lastId = _generator.GenerateId(_lastId);
            user.Id = _lastId;
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

            User removingUser;
            removingUser = _users.FirstOrDefault(u => u.Id == id);
            return RemoveUser(removingUser);
        }

        public bool Remove(User user)
        {
            if (ReferenceEquals(user, null))
            {
                throw new ArgumentNullException(nameof(user));
            }

            User removingUser;
            removingUser = _users.FirstOrDefault(u => u.Equals(user));
            return RemoveUser(removingUser);
        }

        public bool Remove(Predicate<User> match)
        {
            if (ReferenceEquals(match, null))
            {
                throw new ArgumentNullException(nameof(match));
            }

            User removingUser;
            removingUser = _users.FirstOrDefault(u => match(u));
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

        public void Update(User user)
        {
            User updatedUser;
            updatedUser = _users.Find(u => u.Id == user.Id);

            updatedUser.DateOfBirth = user.DateOfBirth;
            updatedUser.FirstName = user.FirstName;
            updatedUser.LastName = user.LastName;
        }

        public void UpdateAll(IEnumerable<User> users)
        {
            if (ReferenceEquals(users, null))
            {
                throw new ArgumentNullException(nameof(users));
            }

            foreach (User item in users)
            {
                Update(item);
            }
        }

        public User Find(Predicate<User> match)
        {
            return _users.FirstOrDefault(u => match(u));
        }

        public User Find(User user)
        {
            CheckUser(user);

            return _users.FirstOrDefault(u => u.Equals(user));
        }

        public IEnumerable<User> GetAll()
        {
            return _users.Select(u => u);
        }

        public List<User> FindAll(Predicate<User> match)
        {
            if (ReferenceEquals(match, null))
            {
                throw new ArgumentNullException(nameof(match));
            }

            return _users.FindAll(match);
        }

        public void Save()
        {
            _saver.Save(_users);
        }

        public void Load()
        {
            _users = _saver.Load().ToList();
            _lastId = _users.Max(u => u.Id);
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

        [Serializable]
        private class DefaultIdGenerator : IIdGenerator
        {
            public int GenerateId(int lastId) => ++lastId;
        }
    }
}