﻿using System.Collections.Generic;
using System.ServiceModel;

namespace WcfServiceLibrary.Interfaces
{
    [ServiceContract]
    public interface ISlaveService
    {
        [OperationContract]
        UserDataContract Find(int slaveNumber, UserDataContract user);

        [OperationContract]
        UserDataContract FindById(int slaveNumber, int id);

        [OperationContract]
        IEnumerable<UserDataContract> GetAll(int slaveNumber);

        [OperationContract]
        IEnumerable<UserDataContract> FindByLastName(int slaveNumber, UserDataContract user);

        [OperationContract]
        IEnumerable<UserDataContract> FindByFirstName(int slaveNumber, UserDataContract user);
    }
}
