using System.Collections.Generic;
using System.ServiceModel;

namespace WcfServiceLibrary.Interfaces
{
    [ServiceContract]
    public interface IMasterService
    {
        [OperationContract]
        int Add(UserDataContract user);

        [OperationContract]
        void AddRange(IEnumerable<UserDataContract> collection);

        [OperationContract]
        void Update(UserDataContract user);

        [OperationContract]
        void UpdateAll(IEnumerable<UserDataContract> users);

        [OperationContract]
        void Load();

        [OperationContract]
        void Save();

        [OperationContract]
        UserDataContract FindById(int id);

        [OperationContract]
        UserDataContract Find(UserDataContract user);

        [OperationContract]
        IEnumerable<UserDataContract> FindByFirstName(UserDataContract user);

        [OperationContract]
        IEnumerable<UserDataContract> FindByLastName(UserDataContract user);

        [OperationContract]
        IEnumerable<UserDataContract> GetAll();
    }
}
