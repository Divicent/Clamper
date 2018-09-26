using System.Linq;
using Clamper.Core.Infrastructure.Filters.Abstract;

namespace Clamper.Core.Infrastructure.Querying.Strategies
{
    public sealed class MysqlQueryStrategy : QueryStrategy
    {
        internal override string Enclose(string str) => $"`{str}`";

        internal override string Select(IRepoQuery query, bool isCount, QueryBuilder queryBuilder)
        {
            return
                $"SELECT {(isCount ? "COUNT(*)" : queryBuilder.CreateSelectColumnList(query.Columns.ToList(), query.Target))} FROM {query.Target}";
        }

        internal override string Page(IRepoQuery query)
        {
            if (query.Page != null && query.PageSize != null)
            {
                return $" LIMIT {query.Page * query.PageSize}, {query.PageSize} ";
            }

            if (query.Skip != null || query.Take != null || query.Limit != null)
            {
                return $" LIMIT {query.Skip ?? 0},  {query.Take ?? query.Limit ?? 0} ";
            }

            return null;
        }

        internal override string GetId()
        {
            return "SELECT LAST_INSERT_ID()";
        }
    }
}
