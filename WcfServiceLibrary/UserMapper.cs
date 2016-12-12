using MyServiceLibrary;

namespace WcfServiceLibrary
{
    public static class UserMapper
    {
        public static User ToUser(this UserDataContract dataContract)
        {
            return new User
            {
                Id = dataContract.Id,
                FirstName = dataContract.FirstName,
                LastName = dataContract.FirstName,
                DateOfBirth = dataContract.DateOfBirth
            };
        }

        public static UserDataContract ToUserDataContract(this User user)
        {
            return new UserDataContract
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth
            };
        }
    }
}
