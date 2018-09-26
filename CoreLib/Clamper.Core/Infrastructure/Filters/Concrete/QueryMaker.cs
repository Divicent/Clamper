using System;

namespace Clamper.Core.Infrastructure.Filters.Concrete
{
    internal static class QueryMaker
    {
        internal static FilterExpression EqualsTo(string propertyName, object value, bool quote)
            => Get(propertyName, FilterExpressionType.EqualsTo, value, quote);

        internal static FilterExpression NotEquals(string propertyName, object value, bool quote)
            => Get(propertyName, FilterExpressionType.NotEqualsTo, value, quote);

        internal static FilterExpression Contains(string propertyName, object value)
            => Get(propertyName, FilterExpressionType.Contains, value, true);

        internal static FilterExpression NotContains(string propertyName, object value)
            => Get(propertyName, FilterExpressionType.NotContains, value, true);

        internal static FilterExpression StartsWith(string propertyName, object value)
            => Get(propertyName, FilterExpressionType.StartsWith, value, true);

        internal static FilterExpression NotStartsWith(string propertyName, object value)
            => Get(propertyName, FilterExpressionType.NotStartsWith, value, true);

        internal static FilterExpression EndsWith(string propertyName, object value)
            => Get(propertyName, FilterExpressionType.EndsWith, value, true);

        internal static FilterExpression NotEndsWith(string propertyName, object value)
            => Get(propertyName, FilterExpressionType.NotEndsWith, value, true);

        internal static FilterExpression IsEmpty(string propertyName)
            => Get(propertyName, FilterExpressionType.IsEmpty, null, false);

        internal static FilterExpression IsNotEmpty(string propertyName)
            => Get(propertyName, FilterExpressionType.IsNotEmpty, null, false);
      
        internal static FilterExpression IsNull(string propertyName)
            => Get(propertyName, FilterExpressionType.IsNull, null, false);
        
        internal static FilterExpression IsNotNull(string propertyName)
            => Get(propertyName, FilterExpressionType.IsNotNull, null, false);

        internal static FilterExpression GreaterThan(string propertyName, object value, bool quote)
            => Get(propertyName, FilterExpressionType.GreaterThan, value, quote);

        internal static FilterExpression LessThan(string propertyName, object value, bool quote)
            => Get(propertyName, FilterExpressionType.LessThan, value, quote);

        internal static FilterExpression GreaterThanOrEquals(string propertyName, object value, bool quote)
            => Get(propertyName, FilterExpressionType.GreaterThanOrEquals, value, quote);
        
        internal static FilterExpression LessThanOrEquals(string propertyName, object value, bool quote)
            => Get(propertyName, FilterExpressionType.LessThanOrEqual, value, quote);

        internal static FilterExpression Between(string propertyName, object from, object to, bool quote)
            => Get(propertyName, FilterExpressionType.Between, new Tuple<object, object>(from, to), quote);

        internal static FilterExpression IsTrue(string propertyName)
            => Get(propertyName, FilterExpressionType.IsTrue, null, false);
        
        internal static FilterExpression IsFalse(string propertyName)
            => Get(propertyName, FilterExpressionType.IsFalse, null, false);

        internal static FilterExpression In(string propertyName, object[] values, bool quote)
            => Get(propertyName, FilterExpressionType.In, values, quote);

        internal static FilterExpression NotIn(string propertyName, object[] values, bool quote)
            => Get(propertyName, FilterExpressionType.NotIn, values, quote);

        private static FilterExpression Get(string colum, FilterExpressionType type, object value, bool quote) => new FilterExpression(colum, type, value, quote);
    }
    
}



