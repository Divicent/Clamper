using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Clamper.Core.Infrastructure.Filters.Abstract;
using Clamper.Core.Infrastructure.Filters.Concrete;
using Clamper.Core.Infrastructure.Models;
using Cache = Clamper.Core.Infrastructure.Querying.QueryBuilderCache;

namespace Clamper.Core.Infrastructure.Querying
{
    public class QueryBuilder
    {
        private readonly IRepoQuery _repoQuery;
	    private readonly StringBuilder _builder;

        private readonly QueryStrategy _strategy;

        private string _select;
	    private string _where;
	    private string _order;
	    private string _page;
        
        internal QueryBuilder(IRepoQuery repoQuery, QueryStrategy strategy)
        {
	        _repoQuery = repoQuery;
	        _builder = new StringBuilder();
            _strategy = strategy;
        }

        internal string Get()
        {
	        return Select(false)
		    .Where()
		    .Order()
		    .Page()
		    .Build();
        }

	    internal string Count()
	    {
		    return Select(true)
			    .Where()
			    .Page()
			    .Build();
	    }
	    
	    internal string SumBy(string name)
	    {
		    return SelectSum(name)
			    .Where()
			    .Page()
			    .Build();
	    }

	    internal string Insert(BaseModel entityToInsert)
	    {
	        var parameters = Cache.GetInsertParameters(entityToInsert, _strategy);
	        var identityProperties = Cache.IdentityPropertiesCache(entityToInsert.GetType());
	        return
	            $"INSERT INTO {parameters.name} ({parameters.columnList}) VALUES ({parameters.parametersList}); {(identityProperties.Any() ? GetId() : "")}";
	    }
	    
	    internal string GetId()
	    {
		    return _strategy.GetId();
	    }

	    internal string Delete(BaseModel entity)
	    {
		    if (entity == null)
		    {
			    throw new ArgumentException("The entity is null, cannot delete a null entity", nameof(entity));
		    }

		    var type = entity.GetType();
		    var keyProperties = Cache.KeyPropertiesCache(type).ToList();

		    if (!keyProperties.Any())
			    throw new ArgumentException("Entity must have at least one [Key] property");

		    var name = Cache.GetTableName(type);

		    var sb = new StringBuilder();
		    sb.Append($"delete from {name} where ");

		    for (var i = 0; i < keyProperties.Count; i++)
		    {
			    var property = keyProperties.ElementAt(i);
			    sb.Append($"{_strategy.Enclose(property.Name)} = @{property.Name}");
			    if (i < keyProperties.Count - 1)
				    sb.Append(" and ");
		    }

		    return sb.ToString();
	    }
	    
	    
	    internal string Update(BaseModel entityToUpdate)
	    {
		    if (entityToUpdate.__DatabaseModelStatus != ModelStatus.Retrieved)
			    return null;

		    if (entityToUpdate.__UpdatedProperties == null || entityToUpdate.__UpdatedProperties.Count < 1)
			    return null;

		    var type = entityToUpdate.GetType();

		    var keyProperties = Cache.KeyPropertiesCache(type).ToList();
		    if (!keyProperties.Any())
			    throw new ArgumentException("Entity must have at least one [Key] property");

		    var name =  Cache.GetTableName(type);

		    var sb = new StringBuilder();
		    sb.Append($"update {name} set ");

		    var allProperties =  Cache.TypePropertiesCache(type);
		    var nonIdProps = allProperties.Where(a => !keyProperties.Contains(a) && entityToUpdate.__UpdatedProperties.Contains(a.Name)).ToList(); // Only updated properties


		    for (var i = 0; i < nonIdProps.Count; i++)
		    {
			    var property = nonIdProps.ElementAt(i);
			    sb.Append($"{_strategy.Enclose(property.Name)} = @{property.Name}");
			    if (i < nonIdProps.Count - 1)
				    sb.Append(", ");
		    }

		    sb.Append(" where ");
		    for (var i = 0; i < keyProperties.Count; i++)
		    {
			    var property = keyProperties.ElementAt(i);
			    sb.Append($"{_strategy.Enclose(property.Name)} = @{property.Name}");
			    if (i < keyProperties.Count - 1)
				    sb.Append(" and ");
		    }

		    return sb.ToString();
	    }

	    internal string WhereClause()
	    {
		    return Where()
			    .Build();
	    }

	    private QueryBuilder Select(bool isCount)
	    {
	        _select = _strategy.Select(_repoQuery, isCount, this);
		   return this;
	    }

	    private QueryBuilder SelectSum(string name)
	    {
		    _select = $"select sum({name}) from {_repoQuery.Target}";
		    return this;
	    }

	    private QueryBuilder Where()
	    {
		    var where = _repoQuery.Where == null ? new Queue<FilterExpression>() : new Queue<FilterExpression>(_repoQuery.Where);

		    if (where.Count <= 0) return this;
		    var builder = new StringBuilder();
		    builder.Append(" where ");

		    var first = true;
		    FilterExpression previous = null;

		    while (where.Count > 0)
		    {
			    var current = where.Dequeue();

			    if (AndOrOr(current))
			    {
				    if (first)
				    {
					    first = false;
					    previous = current;
					    continue;
				    }

				    if (AndOrOr(previous))
				    {
					    previous = current;
					    continue;
				    }

				    previous = current;
				    builder.Append($" {ProcessExpression(current)} ");
			    }
			    else if (current.Type == FilterExpressionType.End || current.Type == FilterExpressionType.Start)
			    {
				    if (current.Type == FilterExpressionType.Start && !first && !AndOrOr(previous))
					    builder.Append(" and ");

				    previous = current;
				    builder.Append($" {ProcessExpression(current)} ");
			    }
			    else
			    {
				    if (!first && previous != null && previous.Type != FilterExpressionType.Start && previous.Type != FilterExpressionType.End && !AndOrOr(previous))
				    {
					    builder.Append(" and ");
				    }

				    previous = current;
				    builder.Append($" {ProcessExpression(current)} ");
			    }

			    first = false;
		    }
		    _where = builder.ToString();

		    return this;
	    }

	    private QueryBuilder Order()
	    {
		    var order = _repoQuery.Order == null ? new Queue<OrderExpression>() : new Queue<OrderExpression>(_repoQuery.Order);

		    if (order.Count <= 0) 
			    return this;
		    
		    var builder = new StringBuilder();
		    builder.Append(" order by ");
		    while (order.Count > 0)
		    {
			    var item = order.Dequeue();
			    builder.Append($" {_strategy.Enclose(item.Column)} {(item.Type == OrderType.Ascending ? "ASC" : "DESC")},");
		    }
		    _order = builder.ToString().TrimEnd(',');

		    return this;
	    }

	    private QueryBuilder Page()
	    {
	        _page = _strategy.Page(_repoQuery);
		    return this;
	    }

	    private string Build()
	    {
		    if (_select != null)
			    _builder.AppendLine(_select);
		    
		    if(_where != null)
			    _builder.AppendLine(_where);
		    
		    if(_order != null)
			    _builder.AppendLine(_order);

		    if (_page != null)
			    _builder.AppendLine(_page);

	        return _builder.ToString();
	    }

	    private static bool AndOrOr(FilterExpression exp)
	    {
		    return exp.Type == FilterExpressionType.And || exp.Type == FilterExpressionType.Or;
	    }

	    internal string CreateSelectColumnList(IReadOnlyCollection<string> columnNames, string target)
	    {
		    if (string.IsNullOrWhiteSpace(target) || columnNames == null || columnNames.Count < 1)
			    return "*";

		    if (Cache.SelectParts.TryGetValue(target, out var result))
			    return result;

		    var builder = new StringBuilder();
		    var first = true;
		    foreach (var columnName in columnNames)
		    {
			    builder.Append($"{(!first ? ", " : "")}{_strategy.Enclose(columnName)}");
			    first = false;
		    }
		    
		    return Cache.SelectParts[target] = builder.ToString();
	    }

        private string ProcessExpression(FilterExpression expression)
        {
            string Quote(object value) => expression.Quote ? $"'{value}'": value.ToString();
            var column = _strategy.Enclose(expression.Column);

            switch (expression.Type)
            {
                case FilterExpressionType.And:
                    return "and";
                case FilterExpressionType.Or:
                    return "or";
                case FilterExpressionType.Column:
                    return "";
                case FilterExpressionType.Start:
                    return "(";
                case FilterExpressionType.End:
                    return ")";
                case FilterExpressionType.EqualsTo:
                    return $"{column} = {Quote(expression.Value)}";
                case FilterExpressionType.NotEqualsTo:
                    return $"{column} != {Quote(expression.Value)}";
                case FilterExpressionType.Contains:
                    return $"{column} LIKE '%{expression.Value}%'";
                case FilterExpressionType.NotContains:
                    return $"{column} NOT LIKE '%{expression.Value}%'";
                case FilterExpressionType.StartsWith:
                    return $"{column} LIKE '{expression.Value}%'";
                case FilterExpressionType.NotStartsWith:
                    return $"{column} NOT LIKE '{expression.Value}%'";
                case FilterExpressionType.EndsWith:
                    return $"{column} LIKE '%{expression.Value}'";
                case FilterExpressionType.NotEndsWith:
                    return $"{column} NOT LIKE '%{expression.Value}'";
                case FilterExpressionType.IsEmpty:
                    return $"{column} = ''";
                case FilterExpressionType.IsNotEmpty:
                    return $"{column} != ''";
                case FilterExpressionType.IsNull:
                    return $"{column} IS NULL";
                case FilterExpressionType.IsNotNull:
                    return $"{column} IS NOT NULL";
                case FilterExpressionType.GreaterThan:
                    return $"{column} > {Quote(expression.Value)}";
                case FilterExpressionType.LessThan:
                    return $"{column} < {Quote(expression.Value)}";
                case FilterExpressionType.GreaterThanOrEquals:
                    return $"{column} >= {Quote(expression.Value)}";
                case FilterExpressionType.LessThanOrEqual:
                    return $"{column} <= {Quote(expression.Value)}";
                case FilterExpressionType.Between:
                    return expression.Value is Tuple<object, object> value
                        ? $"{column} >= {Quote(value.Item1)} AND {column} <= {Quote(value.Item2)}"
                        : "";
                case FilterExpressionType.IsTrue:
                    return $"{column} = 1";
                case FilterExpressionType.IsFalse:
                    return $"{column} = 0";
                case FilterExpressionType.In:
                    return expression.Value is IEnumerable<object> values 
                        ? $"{column} IN({values.Aggregate("", (c, n) => $"{c},{Quote(n)}").TrimStart(',')})" 
                        : "";
                case FilterExpressionType.NotIn:
                    return expression.Value is IEnumerable<object> vals
                        ? $"{column} NOT IN({vals.Aggregate("", (c, n) => $"{c},{Quote(n)}").TrimStart(',')})"
                        : "";
                default:
                    return null;
            }
        }
    }
}

