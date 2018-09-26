using System.Collections.Generic;
using Clamper.Core.Infrastructure.Filters.Concrete;

namespace Clamper.Core.Infrastructure.Filters.Abstract
{
  
   /// <summary>
   /// A filter context is used to build the where clause of the target query
   /// </summary>
	public interface IFilterContext
	{

        /// <summary>
        /// Current expressions as a Queue
        /// </summary>
		Queue<FilterExpression> Expressions { get; set; }
		
        /// <summary>
        /// Adds an and condition
        /// </summary>
        void And();

        /// <summary>
        /// Adds an or condition
        /// </summary>
		void Or();
		
        /// <summary>
        /// Adds a custom expression
        /// </summary>
        /// <param name="expression">Expression to apply</param>
        void Add(FilterExpression expression);

        /// <summary>
        /// Current expressions as a Queue
        /// </summary>
		Queue<FilterExpression> GetFilterExpressions();

        /// <summary>
        /// Starts a scope inside the query (paranthezes)
        /// </summary>
        void StartScope();

        /// <summary>
        /// Ends a scope inside the query (paranthezes)
        /// </summary>
        void EndScope();
	
  }
}

