using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TddDemo.Security.Tests.AuthorizationServiceTests
{
    public class FakeRepository<T> : IRepository<T>
    {
        private IList<T> _list;

        public FakeRepository()
        {
            _list = new List<T>();
        }

        public void Save(T entity)
        {
            _list.Add(entity);
        }

        public void DeleteAll()
        {
            _list.Clear();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.AsQueryable().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _list.AsQueryable().GetEnumerator();
        }

        public Type ElementType
        {
            get { return _list.AsQueryable().ElementType; }
        }

        public System.Linq.Expressions.Expression Expression
        {
            get { return _list.AsQueryable().Expression; }
        }

        public IQueryProvider Provider
        {
            get { return _list.AsQueryable().Provider; }
        }
    }
}
