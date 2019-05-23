using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UboConfigTool.Infrastructure.DomainBase;

namespace UboConfigTool.Infrastructure.Repository
{
    public interface IRepository<T> where T : IAggregateRoot
    {
        T FindBy( object key );
        IList<T> FindAll();
        void Add( T item );
        T this[object key]
        {
            get;
            set;
        }
        void Remove( T item );
    }
}
