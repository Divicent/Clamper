using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Clamper.Core.Infrastructure.Interfaces;
using Clamper.Core.Mapper;

namespace Clamper.Core.Infrastructure
{
	public class ProcedureContainer: IProcedureContainer
    {
		private IDBContext Context { get; }

		internal ProcedureContainer(IDBContext context)
		{
		    Context = context;
		}

		public void Execute(string  name, object parameters) 
		{
			using(var connection = Context.GetConnection())
			{
				connection.Open();
				connection.Execute(name, parameters, commandType: CommandType.StoredProcedure);
			}
		}

        public T QuerySingle<T>(string  name, object parameters) 
		{
			using(var connection = Context.GetConnection())
			{
				connection.Open();
				return connection.QueryFirstOrDefault<T>(name, parameters, commandType: CommandType.StoredProcedure);
			}
		}

        public IEnumerable<T> QueryList<T>(string  name, object parameters) 
		{
			using(var connection = Context.GetConnection())
			{
				connection.Open();
				return connection.Query<T>(name, parameters, commandType: CommandType.StoredProcedure);
			}
		}

        public async Task ExecuteAsync(string  name, object parameters) 
		{
			using(var connection = Context.GetConnection())
			{
				connection.Open();
				await connection.ExecuteAsync(name, parameters, commandType: CommandType.StoredProcedure);
				connection.Close();
			}
		}

        public async Task<T> QuerySingleAsync<T>(string  name, object parameters) 
		{
			using(var connection = Context.GetConnection())
			{
				connection.Open();
				return await connection.QueryFirstOrDefaultAsync<T>(name, parameters, commandType: CommandType.StoredProcedure);
			}
		}

        public async Task<IEnumerable<T>> QueryListAsync<T>(string  name, object parameters) 
		{
			using(var connection = Context.GetConnection())
			{
				connection.Open();
				return await connection.QueryAsync<T>(name, parameters, commandType: CommandType.StoredProcedure);
			}
		}
    }
}

