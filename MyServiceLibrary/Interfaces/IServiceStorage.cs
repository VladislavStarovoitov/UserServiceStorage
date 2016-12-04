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
        void Update<TKey>(Func<T, TKey> keySelector, T item);
        void UpdateAll<TKey>(Func<T, TKey> keySelector, IEnumerable<T> items);
        T Find(T user);
        List<T> FindAll(Predicate<T> match);
        T Find<TKey>(Func<T, TKey> keySelector, TKey key);
        void Save();
        void Load();
    }
}
