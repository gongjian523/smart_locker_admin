using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UboConfigTool.Infrastructure.DomainBase
{
    /// <summary>
    /// This is a marker interface that indicates that an 
    /// Entity is an Aggregate Root.
    /// </summary>
    public interface IAggregateRoot : IEntity
    {
    }
}
