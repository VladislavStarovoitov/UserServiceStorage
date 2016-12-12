using System.Collections.Generic;

namespace MyServiceLibrary.Interfaces
{
    public interface ISaver<T>
    {
        void Save(IEnumerable<T> items);
        IEnumerable<T> Load();
    }
}
