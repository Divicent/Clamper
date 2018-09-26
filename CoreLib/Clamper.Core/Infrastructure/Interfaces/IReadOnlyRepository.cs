using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Clamper.Core.Infrastructure.Filters.Abstract;

namespace Clamper.Core.Infrastructure.Interfaces
{
    /// <summary>
    /// A read only repository only contains methods to get data from  the data source.
    /// </summary>
    /// <typeparam name="T">Object type of the repository</typeparam>
	public interface IReadOnlyRepository<T>
		where T : class
	{
        /// <summary>
        /// Current context
        /// </summary>
		IDBContext Context { get; }

        /// <summary>
        /// Get Executes given query on repository
        /// </summary>
        /// <param name="query">Query to use</param>
        /// <returns>Collection of <typeparamref name="T"/> </returns>
        IEnumerable<T> Get(IRepoQuery query);

        /// <summary>
        /// Asynchronously executes given query on repository
        /// </summary>
        /// <param name="query">Query to use</param>
        /// <returns>Collection of <typeparamref name="T"/> </returns>
        Task<IEnumerable<T>> GetAsync(IRepoQuery query);
		
        /// <summary>
        /// Get the first occurrence or null if the result is empty from the query
        /// </summary>
        /// <param name="query">Query to execute in the repository</param>
        /// <returns>an object with type <typeparamref name="T"/></returns>
        T GetFirstOrDefault(IRepoQuery query);


        /// <summary>
        /// Asynchronously get the first occurrence or null if the result is empty from the query
        /// </summary>
        /// <param name="query">Query to execute in the repository</param>
        /// <returns>an object with type <typeparamref name="T"/></returns>
        Task<T> GetFirstOrDefaultAsync(IRepoQuery query);

        /// <summary>
        /// Executes given query on the repository and returns count of the result set
        /// </summary>
        /// <param name="query">Query to execute in the repository</param>
        /// <returns>an integer</returns>
	    int Count(IRepoQuery query);

        /// <summary>
        /// Asynchronously executes given query on the repository and returns count of the result set
        /// </summary>
        /// <param name="query">Query to execute in the repository</param>
        /// <returns>an integer</returns>
	    Task<int> CountAsync(IRepoQuery query);

        /// <summary>
        /// Get sum of a column
        /// </summary>
        /// <param name="query">Query to execute in the repository</param>
        /// <returns>given type</returns>
        TA SumBy<TA>(IRepoQuery query, string column);


        /// <summary>
        /// Get sum of a column asynchronously
        /// </summary>
        /// <param name="query">Query to execute in the repository</param>
        /// <returns>given type</returns>
        Task<TA> SumByAsync<TA>(IRepoQuery query, string column);

        /// <summary>
        /// Extracts the where clause of the provided query object
        /// </summary>
        /// <param name="query">Query to use</param>
        /// <returns>The where clause as a string</returns>
		string GetWhereClause(IRepoQuery query);
	}
}

