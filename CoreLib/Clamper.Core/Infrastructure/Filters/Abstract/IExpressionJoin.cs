namespace Clamper.Core.Infrastructure.Filters.Abstract
{

    /// <summary>
    /// An expression join is a helper that helps to continue a query in a builder-like pattern
    /// </summary>
    /// <typeparam name="T">Type of the filter context</typeparam>
    /// <typeparam name="TQ">Type of the query context</typeparam>
    public interface IExpressionJoin<out T, out TQ> where T : IFilterContext
    {

        /// <summary>
        /// Adds an and condition to the query
        /// </summary>
        T And { get; }

        /// <summary>
        /// Adds an or condition to the query
        /// </summary>
        T Or { get; }
        
        /// <summary>
        /// Completes the filter context and returns to the current query context
        /// </summary>
        TQ Filter();
        
        /// <summary>
        /// Starts Parenthesizes
        /// </summary>
        IExpressionJoin<T, TQ> Start { get; }

        /// <summary>
        /// Ends Parenthesizes
        /// </summary>
        IExpressionJoin<T, TQ> End { get; }
    }
}


