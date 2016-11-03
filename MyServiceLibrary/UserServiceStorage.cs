﻿using System;
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
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (user.FirstName == null)
            {
                throw new InvalidUserException(nameof(user.FirstName));
            }

            if (user.LastName == null)
            {
                throw new InvalidUserException(nameof(user.LastName));
            }

            if (user.DateOfBirth < DateTime.Now)
            {
                throw new InvalidUserException(nameof(user.DateOfBirth));
            }

            user.Id = _generator.GenerateId(_lastId);
            _users.Add(user);
            return user.Id;
        }

        public bool Remove(int id)
        {
            if (id < 0 && id > _lastId)
            {
                return false;
            }

            var removingUser = _users.FirstOrDefault(u => u.Id == id);
            if (ReferenceEquals(removingUser, null))
            {
                return false;
            }

            _users.Remove(removingUser);
            return true;
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
    }
}
