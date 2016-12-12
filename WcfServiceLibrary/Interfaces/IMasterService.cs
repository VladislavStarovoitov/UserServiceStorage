using System;
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
        IEnumerable<UserDataContract> FindByFirstName(string firstName);

        [OperationContract]
        IEnumerable<UserDataContract> FindByLastName(string lastName);

        [OperationContract]
        IEnumerable<UserDataContract> FindByDateOfBirth(DateTime dateOfBirth);

        [OperationContract]
        IEnumerable<UserDataContract> GetAll();

        [OperationContract]
        bool Remove(UserDataContract user);

        [OperationContract]
        bool RemoveById(int id);
    }
}
