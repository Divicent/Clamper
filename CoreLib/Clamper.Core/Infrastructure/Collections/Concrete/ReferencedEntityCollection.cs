using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Clamper.Core.Infrastructure.Actions.Abstract;
using Clamper.Core.Infrastructure.Actions.Concrete;
using Clamper.Core.Infrastructure.Collections.Abstract;
using Clamper.Core.Infrastructure.Models;

namespace Clamper.Core.Infrastructure.Collections.Concrete
{
    public class ReferencedEntityCollection<T> : IReferencedEntityCollection<T> where T: BaseModel
	{
		private readonly List<T> _collection;
		private readonly Action<object> _addAction;
        private readonly BaseModel _creator;

		public ReferencedEntityCollection(IEnumerable<T> collection, Action<object> addAction, BaseModel creator)
		{
			_collection = collection.ToList();
			_addAction = addAction;
            _creator = creator;
		}

		public void Add(T entityToAdd) 
		{
            if (entityToAdd == null)
                return;
            switch (_creator.__DatabaseModelStatus)
            {
                case ModelStatus.Retrieved:
                    _addAction(entityToAdd);
                    break;
                case ModelStatus.ToAdd:
                    if(_creator.__ActionsToRunWhenAdding == null)
                        _creator.__ActionsToRunWhenAdding = new List<IAddAction>();
                    _creator.__ActionsToRunWhenAdding.Add(new AddAction(_addAction, entityToAdd));
                    break;
                case ModelStatus.JustInMemory:
                case ModelStatus.Deleted:
                    break;
                default:
                    break;
            }    
			_collection.Add(entityToAdd);
		}

        public IEnumerator<T> GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _collection.GetEnumerator();
        }
    }
} 


