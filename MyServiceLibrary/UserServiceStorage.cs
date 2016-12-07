using MyServiceLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Threading;

namespace MyServiceLibrary
{
    public class UserServiceStorage : MarshalByRefObject, IEnumerable<User>, IServiceStorage<User>
    {
        private int _lastId = 0;
        private List<User> _users = new List<User>();
        private IIdGenerator _generator;
        private ISaver<User> _saver;
        private ReaderWriterLockSlim _locker = new ReaderWriterLockSlim();

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
            _locker.EnterWriteLock();
            try
            {
                _users.Add(user);
            }
            finally
            {
                _locker.ExitWriteLock();
            }            
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
            _locker.EnterReadLock();
            try
            {
                removingUser = _users.FirstOrDefault(u => u.Id == id);
            }
            finally
            {
                _locker.ExitReadLock();
            }
            return RemoveUser(removingUser);
        }

        public bool Remove(User user)
        {
            if (ReferenceEquals(user, null))
            {
                throw new ArgumentNullException(nameof(user));
            }

            User removingUser;
            _locker.EnterReadLock();
            try
            {
                removingUser = _users.FirstOrDefault(u => u.Equals(user));
            }
            finally
            {
                _locker.ExitReadLock();
            }
            return RemoveUser(removingUser);
        }

        public int RemoveAll(Predicate<User> match)
        {
            if (ReferenceEquals(match, null))
            {
                throw new ArgumentNullException(nameof(match));
            }

            _locker.EnterWriteLock();
            try
            {
                return _users.RemoveAll(match);
            }
            finally
            {
                _locker.ExitWriteLock();
            }
        }

        public void Update(User user)
        {
            User updatedUser;
            _locker.EnterReadLock();
            try
            {
                updatedUser = _users.Find(u => u.Id == user.Id);
            }
            finally
            {
                _locker.ExitReadLock();
            }

            _locker.EnterWriteLock();
            try
            {
                updatedUser.DateOfBirth = user.DateOfBirth;
                updatedUser.FirstName = user.FirstName;
                updatedUser.LastName = user.LastName;
            }
            finally
            {
                _locker.ExitWriteLock();
            }
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
            _locker.EnterReadLock();
            try
            {
                return _users.FirstOrDefault(u => match(u));
            }
            finally
            {
                _locker.ExitReadLock();
            }
        }

        public User Find(User user)
        {
            CheckUser(user);

            _locker.EnterReadLock();
            try
            {
                return _users.FirstOrDefault(u => u.Equals(user));
            }
            finally
            {
                _locker.ExitReadLock();
            }
        }

        public IEnumerable<User> GetAll()
        {
            _locker.EnterReadLock();
            try
            {
                return _users.Select(u => u);
            }
            finally
            {
                _locker.ExitReadLock();
            }
        }

        public List<User> FindAll(Predicate<User> match)
        {
            if (ReferenceEquals(match, null))
            {
                throw new ArgumentNullException(nameof(match));
            }

            List<User> list = new List<User>();
            _locker.EnterReadLock();
            try
            {
                foreach (var item in _users)
                {
                    if (match(item))
                    {
                        list.Add(item);
                    }
                }
            }
            finally
            {
                _locker.ExitReadLock();
            }

            return list;
        }

        public void Save()
        {
            _locker.EnterReadLock();
            try
            {
                _saver.Save(_users);
            }
            finally
            {
                _locker.ExitReadLock();
            }
        }

        public void Load()
        {
            List<User> users;
            users = _saver.Load().ToList();
            Interlocked.Exchange(ref _users, users);
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