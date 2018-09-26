using System;
using System.Collections.Generic;
using Clamper.Core.Infrastructure.Filters.Abstract;

namespace Clamper.Core.Infrastructure.Filters.Concrete
{
    public abstract class BaseFilterContext : IFilterContext
    {
        private int _startedScopes;

        protected BaseFilterContext()
        {
            Expressions = new Queue<FilterExpression>();
        }

        public Queue<FilterExpression> Expressions { get; set; }

        public void And()
        {
            Expressions.Enqueue(new FilterExpression(FilterExpressionType.And));
        }

        public void Or()
        {
            Expressions.Enqueue(new FilterExpression(FilterExpressionType.Or));
        }

        public void Add(FilterExpression expression)
        {
            Expressions.Enqueue(expression);
        }

        public Queue<FilterExpression> GetFilterExpressions()
        {
            return Expressions;
        }

        public void StartScope()
        {
            Expressions.Enqueue(new FilterExpression(FilterExpressionType.Start));
            _startedScopes++;
        }

        public void EndScope()
        {
            if(_startedScopes <1 )
                throw new Exception("No scopes are started");

            Expressions.Enqueue(new FilterExpression(FilterExpressionType.End));
            _startedScopes--;
        }
    }
}

