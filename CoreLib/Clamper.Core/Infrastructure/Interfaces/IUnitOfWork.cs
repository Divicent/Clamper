using System;
using System.Collections.Concurrent;
using System.Data;
using System.Threading.Tasks;
using Clamper.Core.Infrastructure.Models;

namespace Clamper.Core.Infrastructure.Interfaces
{
    /// <summary>
    /// A unit of work is a collection of operations on a data source. operations could be updates, deletes, inserts
    /// </summary>
	public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Starts a transaction on this unit of work
        /// </summary>
        /// <returns>A new transaction</returns>
        IDbTransaction BeginTransaction();

        IDBContext Context { get; }

        ConcurrentDictionary<string, object> Repos { get; set; }

        IProcedureContainer Procedures { get; }

        void AddOp(IOperation operation);
        void AddObj(BaseModel obj);

        /// <summary>
        /// This will commit all changes to the data source performed in this unit of work one by one. including all tracked changes of the objects retrieved from this unit of work
        /// </summary>
        void Commit();

        /// <summary>
        /// This will commit all changes to the data source asynchronously performed in this unit of work one by one. including all tracked changes of the objects retrieved from this unit of work
        /// </summary>
        Task CommitAsync();
    }
}

