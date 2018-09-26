using System;
using System.Linq;
using Clamper.Core.Infrastructure.Filters.Abstract;

namespace Clamper.Core.Infrastructure.Filters.Concrete
{
    public class DateFilter<T, TQ> : IDateFilter<T, TQ> where T : IFilterContext
    {
        private readonly string _propertyName;
        private readonly T _parent;
        private readonly TQ _q;

        public DateFilter(string propertyName, T parent, TQ q)
        {
            _parent = parent;
            _propertyName = propertyName;
            _q = q;
        }

        public IExpressionJoin<T, TQ> EqualsTo(DateTime date)
        {
            _parent.Add(QueryMaker.EqualsTo(_propertyName, date, true));
            return new ExpressionJoin<T, TQ>(_parent, _q);
        }

        public IExpressionJoin<T, TQ> NotEquals(DateTime date)
        {
            _parent.Add(QueryMaker.NotEquals(_propertyName, date, true));
            return new ExpressionJoin<T, TQ>(_parent, _q);
        }

        public IExpressionJoin<T, TQ> LargerThan(DateTime date)
        {
            _parent.Add(QueryMaker.GreaterThan(_propertyName, date, true));
            return new ExpressionJoin<T, TQ>(_parent, _q);
        }

        public IExpressionJoin<T, TQ> LessThan(DateTime date)
        {
            _parent.Add(QueryMaker.LessThan(_propertyName, date, true));
            return new ExpressionJoin<T, TQ>(_parent, _q);
        }

        public IExpressionJoin<T, TQ> LargerThanOrEqualTo(DateTime date)
        {
            _parent.Add(QueryMaker.GreaterThanOrEquals(_propertyName, date, true));
            return new ExpressionJoin<T, TQ>(_parent, _q);
        } 

        public IExpressionJoin<T, TQ> LessThanOrEqualTo(DateTime date)
        {
            _parent.Add(QueryMaker.LessThanOrEquals(_propertyName, date, true));
            return new ExpressionJoin<T, TQ>(_parent, _q);
        }

        public IExpressionJoin<T, TQ> Between(DateTime from, DateTime to)
        {
            _parent.Add(QueryMaker.Between(_propertyName, from, to, true));
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

        public IExpressionJoin<T, TQ> In(params DateTime[] items)
        {
            _parent.Add(QueryMaker.In(_propertyName, items.Cast<object>().ToArray(), true));
            return new ExpressionJoin<T, TQ>(_parent, _q);
        }

        public IExpressionJoin<T, TQ> NotIn(params DateTime[] items)
        {
            _parent.Add(QueryMaker.NotIn(_propertyName, items.Cast<object>().ToArray(), true));
            return new ExpressionJoin<T, TQ>(_parent, _q);
        }
    }
}

