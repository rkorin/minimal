using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IRepository<T> where T : class
    {
        T get(int id);
        IEnumerable<T> get();
        void save(T entity);
        void delete(int id);
    }

}
