using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyServiceLibrary.Interfaces
{
    public interface IServiceStorage<T>
    {
        int Add(T item);
        void AddRange(IEnumerable<T> collection);
        bool Remove(T item);
        int RemoveAll(Predicate<T> match);
        void Update(T item);
        void UpdateAll(IEnumerable<T> items);
        T Find(T item);
        List<T> FindAll(Predicate<T> match);
        T Find(Predicate<T> match);
        IEnumerable<T> GetAll();
        void Save();
        void Load();
    }
}
