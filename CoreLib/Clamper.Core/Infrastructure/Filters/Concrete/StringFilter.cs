using System.Linq;
using Clamper.Core.Infrastructure.Filters.Abstract;

namespace Clamper.Core.Infrastructure.Filters.Concrete
{
    public class StringFilter<T, TQ> : IStringFilter<T, TQ> where T : IFilterContext
    {
        private readonly string _propertyName;
        private readonly T _parent;
        private readonly TQ _q;

        public StringFilter(string propertyName, T parent, TQ q)
        {
            _parent = parent;
            _propertyName = propertyName;
            _q = q;
        }

        public IExpressionJoin<T, TQ> EqualsTo(string str)
        {
            _parent.Add(QueryMaker.EqualsTo(_propertyName, str, true));
            return new ExpressionJoin<T, TQ>(_parent, _q);
        }

        public IExpressionJoin<T, TQ> NotEquals(string str)
        {
            _parent.Add(QueryMaker.NotEquals(_propertyName, str, true));
            return new ExpressionJoin<T, TQ>(_parent, _q);
        }

        public IExpressionJoin<T, TQ> Contains(string str)
        {
            _parent.Add(QueryMaker.Contains(_propertyName, str));
            return new ExpressionJoin<T, TQ>(_parent, _q);
        }

        public IExpressionJoin<T, TQ> StartsWith(string str)
        {
            _parent.Add(QueryMaker.StartsWith(_propertyName, str));
            return new ExpressionJoin<T, TQ>(_parent, _q);
        }

        public IExpressionJoin<T, TQ> NotStartsWith(string str)
        {
            _parent.Add(QueryMaker.NotStartsWith(_propertyName, str));
            return new ExpressionJoin<T, TQ>(_parent, _q);
        }

        public IExpressionJoin<T, TQ> EndsWith(string str)
        {
            _parent.Add(QueryMaker.EndsWith(_propertyName, str));
            return new ExpressionJoin<T, TQ>(_parent, _q);
        }

        public IExpressionJoin<T, TQ> NotEndsWith(string str)
        {
            _parent.Add(QueryMaker.NotEndsWith(_propertyName, str));
            return new ExpressionJoin<T, TQ>(_parent, _q);
        }

        public IExpressionJoin<T, TQ> IsEmpty()
        {
            _parent.Add(QueryMaker.IsEmpty(_propertyName));
            return new ExpressionJoin<T, TQ>(_parent, _q);
        }

        public IExpressionJoin<T, TQ> IsNotEmpty()
        {
            _parent.Add(QueryMaker.IsNotEmpty(_propertyName));
            return new ExpressionJoin<T, TQ>(_parent, _q);
        }

        public IExpressionJoin<T, TQ> IsNull()
        {
            _parent.Add(QueryMaker.IsNull(_propertyName));
            return new ExpressionJoin<T, TQ>(_parent, _q);
        }

        public IExpressionJoin<T, TQ> IsNotNull()
        {
            _parent.Add(QueryMaker.IsNotNull(_propertyName));
            return new ExpressionJoin<T, TQ>(_parent, _q);
        }

		public IExpressionJoin<T, TQ> IsNull(bool isNull)
        {
			_parent.Add(isNull ? QueryMaker.IsNull(_propertyName) : QueryMaker.IsNotNull(_propertyName));
			return new ExpressionJoin<T, TQ>(_parent, _q);
        }

        public IExpressionJoin<T, TQ> In(params string[] items)
        {
            _parent.Add(QueryMaker.In(_propertyName, items.Cast<object>().ToArray(), true));
            return new ExpressionJoin<T, TQ>(_parent, _q);
        }

        public IExpressionJoin<T, TQ> NotIn(params string[] items)
        {
            _parent.Add(QueryMaker.NotIn(_propertyName, items.Cast<object>().ToArray(), true));
            return new ExpressionJoin<T, TQ>(_parent, _q);
        }
    } 
}

