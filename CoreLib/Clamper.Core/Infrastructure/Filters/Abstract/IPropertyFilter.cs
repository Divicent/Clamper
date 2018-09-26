namespace Clamper.Core.Infrastructure.Filters.Abstract
{

    /// <summary>
    /// Holds a filter to be applied on an attribute
    /// </summary>
    public interface IPropertyFilter
    {
        /// <summary>
        /// The property name to apply the filter
        /// </summary>
        string PropertyName { get; set; }

        /// <summary>
        /// Expression type
        /// </summary>
        string Type { get; set; }
            
        /// <summary>
        /// Value if available
        /// </summary>
        object Value { get; set; }
    }
}

