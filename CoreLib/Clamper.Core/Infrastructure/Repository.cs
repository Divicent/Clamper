using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Clamper.Core.Infrastructure.Filters.Abstract;
using Clamper.Core.Infrastructure.Interfaces;
using Clamper.Core.Infrastructure.Models;
using Clamper.Core.Infrastructure.Querying;
using Clamper.Core.Mapper;

namespace Clamper.Core.Infrastructure
{
    public abstract class Repository<T> : IRepository<T>
        where T : BaseModel 
    {
        public IDBContext Context { get;}
        private IUnitOfWork UnitOfWork { get;}

        protected Repository(IDBContext context, IUnitOfWork unitOfWork)
        {
            Context = context;
            UnitOfWork = unitOfWork;
        }

        public virtual void Add(T entity, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "Add to DB null entity");
            }
            
            entity.__DatabaseUnitOfWork = UnitOfWork;           
            var operation = new Operation(OperationType.Add, entity);
            UnitOfWork.AddOp(operation);    
            entity.__DatabaseModelStatus = ModelStatus.ToAdd;  
        }

        public virtual void Add(IEnumerable<T> entities, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities), "Add to DB null entity");
            }
            
            foreach(var entity in entities)
                Add(entity, transaction, commandTimeout);
        }

        public virtual void Remove(T entity, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "Remove in DB null entity");
            }
            
            var operation = new Operation(OperationType.Remove, entity);
            UnitOfWork.AddOp(operation);
        }

        public virtual void Remove(IEnumerable<T> entities, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities), "Remove in DB null entity");
            }

            foreach(var entity in entities)
                Remove(entity, transaction, commandTimeout);
        }

        public virtual IEnumerable<T> Get(IRepoQuery query)
        {
            using (var connection = Context.GetConnection())
            {
                var items = connection.Query<T>(new QueryBuilder(query, Context.QueryStrategy).Get()).ToList();
                foreach (var item in items)
                    AddItemToUnit(item);
                return items;    
            }
        }

        public virtual async Task<IEnumerable<T>> GetAsync(IRepoQuery query)
        {
            using (var connection = Context.GetConnection())
            {
                var items = (await connection.QueryAsync<T>(new QueryBuilder(query, Context.QueryStrategy).Get())).ToList();
                foreach (var item in items)
                    AddItemToUnit(item);
                return items;    
            }
        }

		public virtual T GetFirstOrDefault(IRepoQuery query)
        {
            using (var connection = Context.GetConnection())
            {
                var item = connection.QuerySingleOrDefault<T>(new QueryBuilder(query, Context.QueryStrategy).Get());
                if(item == null)
                    return null;
                AddItemToUnit(item);
                return item;  
            }
        }

        public virtual async Task<T> GetFirstOrDefaultAsync(IRepoQuery query)
        {
            using (var connection = Context.GetConnection())
            {
                var item = await connection.QuerySingleOrDefaultAsync<T>(new QueryBuilder(query, Context.QueryStrategy).Get());
                if(item == null)
                    return null;
                AddItemToUnit(item);
                return item;  
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

		private void AddItemToUnit(T item) 
		{
			item.__DatabaseUnitOfWork = UnitOfWork;
            item.__DatabaseModelStatus = ModelStatus.Retrieved;
            UnitOfWork.AddObj(item);
		}
    }
}

