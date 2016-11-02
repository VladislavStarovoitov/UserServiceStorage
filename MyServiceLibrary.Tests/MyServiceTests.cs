using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MyServiceLibrary.Tests
{
    [TestClass]
    public class MyServiceTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Add_NullUser_ExceptionThrown()
        {
            var service = new UserServiceStorage();

            service.Add(null);
        }

        [TestMethod]
        public void Add_ValidUser_ValidUserIsAdded()
        {
            var service = new UserServiceStorage();

            int id = service.Add(new User
            {
                FirstName = "Vlad",
                LastName = "Star",
                DateOfBirth = DateTime.Now
            });
            Assert.AreEqual(0, id);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidUserException))]
        public void Add_FirstNameIsNull_ExceptionThrown()
        {
            var service = new UserServiceStorage();

            service.Add(new User
            {
                FirstName = null,
                LastName = "Star",
                DateOfBirth = DateTime.Now
            });
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidUserException))]
        public void Add_LastNameIsNull_ExceptionThrown()
        {
            var service = new UserServiceStorage();

            service.Add(new User
            {
                FirstName = "Vlad",
                LastName = null,
                DateOfBirth = DateTime.Now
            });
        }
    }
}
