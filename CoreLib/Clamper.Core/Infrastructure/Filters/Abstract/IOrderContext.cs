using System.Collections.Generic;
using Clamper.Core.Infrastructure.Filters.Concrete;

namespace Clamper.Core.Infrastructure.Filters.Abstract
{
    /// <summary>
    /// An order context is used to build the order by clause of the target query
    /// </summary>
	public interface IOrderContext
    {
        /// <summary>
        /// Adds a custom expression
        /// </summary>
        /// <param name="expression">Expression to apply</param>
        void Add(OrderExpression expression);
        
        /// <summary>
        /// Current expressions as a Queue
        /// </summary>
        Queue<OrderExpression> GetOrderExpressions();
    }
}


