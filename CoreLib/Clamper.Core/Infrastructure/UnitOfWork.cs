using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Clamper.Core.Extensions;
using Clamper.Core.Infrastructure.Interfaces;
using Clamper.Core.Infrastructure.Models;
using Clamper.Core.Infrastructure.Querying;

namespace Clamper.Core.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
		private IProcedureContainer _procedureContainer;

        private readonly List<IOperation> _operations;
        private readonly HashSet<BaseModel> _objects;

		public IProcedureContainer Procedures => _procedureContainer ?? ( _procedureContainer = new ProcedureContainer(Context));

        public IDBContext Context { get;}

        public ConcurrentDictionary<string, object> Repos { get; set; }

        private IDbTransaction Transaction { get; set; }

        public UnitOfWork(IDBContext context)
        {
            Context = context;
            _objects = new HashSet<BaseModel>();
            _operations = new List<IOperation>();
            Repos = new ConcurrentDictionary<string, object>();
        }
            
        public IDbTransaction BeginTransaction()
        {
            if (Transaction != null)
            {
                throw new NullReferenceException("Not finished previous transaction");
            }
            return Transaction;
        }


        public void Commit()
        {
            var updated = _objects.Where(o => o.__UpdatedProperties != null && o.__UpdatedProperties.Count > 0).ToList();

            try
            {
                if (updated.Count > 0)
                    _operations.AddRange(updated.Select(u => new Operation(OperationType.Update, u)));

                if (_operations.Count <= 0)
                    return;

                var toAdd = _operations.Where(o => o.Type == OperationType.Add).ToList();
                var toDelete = _operations.Where(o => o.Type == OperationType.Remove).ToList();
                var toUpdate = _operations.Where(o => o.Type == OperationType.Update).ToList();

                if (toDelete.Count > 0)
                {
                    foreach (var operation in toDelete)
                    {
                        var deleted = Context.GetConnection().Delete(operation.Object, new QueryBuilder(null, Context.QueryStrategy));
                        if (deleted) { operation.Object.__DatabaseModelStatus = ModelStatus.Deleted; }
                    }
                }

                if (toAdd.Count > 0)
                {
                    var queryBuilder = new QueryBuilder(null, Context.QueryStrategy);
                    foreach (var operation in toAdd)
                    {
                        var newId = Context.GetConnection().Insert(operation.Object, queryBuilder);
                        if(newId != null)
                            operation.Object.SetId(newId);
                        operation.Object.__DatabaseModelStatus = ModelStatus.Retrieved;
                        if (operation.Object.__ActionsToRunWhenAdding == null ||
                            operation.Object.__ActionsToRunWhenAdding.Count <= 0) continue;

                        foreach (var addAction in operation.Object.__ActionsToRunWhenAdding)
                            addAction.Run();
                        operation.Object.__ActionsToRunWhenAdding.Clear();
                    }
                }

                if (toUpdate.Count > 0)
                {
                    foreach (var operation in toUpdate)
                    {
                        Context.GetConnection().Update(operation.Object, new QueryBuilder(null, Context.QueryStrategy));
                        operation.Object.__UpdatedProperties.Clear();
                    }
                }
                    
                _operations.Clear();
            }
            catch (Exception e)
            {
                throw new Exception("Unable to commit changes", e);
            }
        }

        public async Task CommitAsync()
        {
            var updated = _objects.Where(o => o.__UpdatedProperties != null && o.__UpdatedProperties.Count > 0).ToList();

            try
            {
                if (updated.Count > 0)
                    _operations.AddRange(updated.Select(u => new Operation(OperationType.Update, u)));

                if (_operations.Count > 0)
                {
                    var toAdd = _operations.Where(o => o.Type == OperationType.Add).ToList();
                    var toDelete = _operations.Where(o => o.Type == OperationType.Remove).ToList();
                    var toUpdate = _operations.Where(o => o.Type == OperationType.Update).ToList();

					if (toDelete.Count > 0)
					{
                    	foreach (var operation in toDelete)
						{
                            var deleted = await Context.GetConnection().DeleteAsync(operation.Object, new QueryBuilder(null, Context.QueryStrategy));
                            if (deleted) { operation.Object.__DatabaseModelStatus = ModelStatus.Deleted; }
                        }
					}

					if (toAdd.Count > 0)
					{
					    var queryBuilder = new QueryBuilder(null, Context.QueryStrategy);
                        foreach (var operation in toAdd)
                        {
                            var newId = await Context.GetConnection().InsertAsync(operation.Object, queryBuilder);
                             if(newId != null)
                                operation.Object.SetId(newId);
                            operation.Object.__DatabaseModelStatus = ModelStatus.Retrieved;

                            if (operation.Object.__ActionsToRunWhenAdding == null ||
                                operation.Object.__ActionsToRunWhenAdding.Count <= 0) continue;

                            foreach (var addAction in operation.Object.__ActionsToRunWhenAdding)
                                addAction.Run();
                            operation.Object.__ActionsToRunWhenAdding.Clear();
                        }
                    }

					if (toUpdate.Count > 0)
					{
						foreach (var operation in toUpdate)
						{
                            await Context.GetConnection().UpdateAsync(operation.Object, new QueryBuilder(null, Context.QueryStrategy));
                            operation.Object.__UpdatedProperties.Clear();
                        }
					}
                    
					_operations.Clear();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Unable to commit changes", e);
            }
        }


        public void Dispose()
        {
            Transaction?.Dispose();
        }

        public void AddOp(IOperation operation)
        {
            _operations.Add(operation);
        }

        public void AddObj(BaseModel obj)
        {
            _objects.Add(obj);
        }    
    }
}

