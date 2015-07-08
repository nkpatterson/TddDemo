using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TddDemo.Security
{
    public interface IRepository<T> : IQueryable<T>
    {
        void Save(T entity);

        void DeleteAll();
    }
}
