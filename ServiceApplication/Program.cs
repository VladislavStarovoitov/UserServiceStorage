using MyServiceLibrary;
using System;
using System.Collections.Generic;

namespace ServiceApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var user = new User
            {
                FirstName = "Vlad",
                LastName = "Star",
                DateOfBirth = DateTime.Now
            };

            var service = new UserServiceStorage(new UserXmlSaver()) { user, user };

            service.Save();
            // 1. Add a new user to the storage.
            // 2. Remove an user from the storage.
            // 3. Search for an user by the first name.
            // 4. Search for an user by the last name.
        }
    }
}
