using System;

namespace MyServiceLibrary
{
    // Rename this class. Give the class an appropriate name that will allow all other developers understand it's purpose.
    public class MyService
    {
        // Add all required methods here.
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
