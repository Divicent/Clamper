using System.Data;
using Clamper.Core.Infrastructure.Querying;

namespace Clamper.Core.Infrastructure.Interfaces
{
    /// <summary>
    /// A system wide context that holds the connection to the database and manages the connection
    /// </summary>
    public interface IDBContext
    {
        /// <summary>
        /// Creates a new unit of work for this context
        /// </summary>
        /// <returns>A new unit of work</returns>
        IUnitOfWork Unit();

        /// <summary>
        /// Get the query strategy
        /// </summary>
        QueryStrategy QueryStrategy { get; }

        /// <summary>
        /// Get a new connection
        /// </summary>
        /// <returns></returns>
        IDbConnection GetConnection();
    }
}

