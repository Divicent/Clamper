using System.Collections.Generic;
using Clamper.Core.Infrastructure.Models;

namespace Clamper.Core.Infrastructure.Collections.Abstract
{
    /// <summary>
    /// Collection of data models retrieved from an object. as a reference collection
    /// </summary>
    /// <typeparam name="T">Type of object</typeparam>
    public interface IReferencedEntityCollection<T> : IEnumerable<T> where T : BaseModel
    {
        /// <summary>
        /// Add an object to the collection. this will also update the parent objects related value which the collection is retrieved from. this will not add this object to the data source.
        /// </summary>
        /// <param name="entityToAdd">entity to add </param>
        void Add(T entityToAdd);
    }
}


