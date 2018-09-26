using System.Collections.Generic;
using System.Threading.Tasks;
using Clamper.Core.Infrastructure.Filters.Abstract;
using Clamper.Core.Infrastructure.Interfaces;
using Clamper.Core.Infrastructure.Querying;
using Clamper.Core.Mapper;

namespace Clamper.Core.Infrastructure
{
    public class ReadOnlyRepository<T> : IReadOnlyRepository<T>
        where T : class
    {
        public IDBContext Context { get;}

        protected ReadOnlyRepository(IDBContext context)
        {
            Context = context;
        }

        public virtual IEnumerable<T> Get(IRepoQuery query)
        {
            using (var connection = Context.GetConnection())
            {
                return connection.Query<T>(new QueryBuilder(query, Context.QueryStrategy).Get());
            }
        }

        public virtual async Task<IEnumerable<T>> GetAsync(IRepoQuery query)
        {
            using (var connection = Context.GetConnection())
            {
                return  await connection.QueryAsync<T>(new QueryBuilder(query, Context.QueryStrategy).Get());
            }
        }

		public virtual T GetFirstOrDefault(IRepoQuery query)
        {
            using (var connection = Context.GetConnection())
            {
                return connection.QuerySingleOrDefault<T>(new QueryBuilder(query, Context.QueryStrategy).Get());
            }
        }

        public virtual async Task<T> GetFirstOrDefaultAsync(IRepoQuery query)
        {
            using (var connection = Context.GetConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<T>(new QueryBuilder(query, Context.QueryStrategy).Get());
            }
        }

        public virtual int Count(IRepoQuery query)
        {
            using (var connection = Context.GetConnection())
            {
                return  connection.ExecuteScalar<int>(new QueryBuilder(query, Context.QueryStrategy).Count());
            }
        }

        public virtual async Task<int> CountAsync(IRepoQuery query)
        {
            using (var connection = Context.GetConnection())
            {
                return await connection.ExecuteScalarAsync<int>(new QueryBuilder(query, Context.QueryStrategy).Count());
            }
        }


        public virtual TA SumBy<TA>(IRepoQuery query, string column)
        {
            using (var connection = Context.GetConnection())
            {
                return  connection.ExecuteScalar<TA>(new QueryBuilder(query, Context.QueryStrategy).SumBy(column));
            }
        }


        public virtual async Task<TA> SumByAsync<TA>(IRepoQuery query, string column)
        {
            using (var connection = Context.GetConnection())
            {
                return await connection.ExecuteScalarAsync<TA>(new QueryBuilder(query, Context.QueryStrategy).SumBy(column));
            }
        }

		public string GetWhereClause(IRepoQuery query) 
		{
			return new QueryBuilder(query, Context.QueryStrategy).WhereClause();
		}
    }
}

