
using System.Linq;
using System.Text;
using Clamper.Core.Infrastructure.Filters.Abstract;

namespace Clamper.Core.Infrastructure.Querying.Strategies
{
    public sealed class MicrosoftSqlServerQueryStrategy: QueryStrategy
    {
        internal override string Enclose(string str) => $"[{str}]";
        internal override string Select(IRepoQuery query, bool isCount, QueryBuilder queryBuilder)
        {
            return string.Format("SELECT {0} {1} FROM " + query.Target, query.Limit != null ? " TOP "
                                                                                              + query.Limit : "", isCount ? "COUNT(*)" : queryBuilder.CreateSelectColumnList(query.Columns.ToList(), query.Target));
        }

        internal override string Page(IRepoQuery query)
        {
            if (query.Page != null && query.PageSize != null)
            {
                return $" OFFSET ({query.Page * query.PageSize}) ROWS " + $" FETCH NEXT {query.PageSize} ROWS ONLY ";
            }

            var builder = new StringBuilder();
            if (query.Skip != null)
                builder.Append($" OFFSET ({query.Skip}) ROWS ");

            if (query.Take != null)
                builder.Append($" FETCH NEXT {query.Take} ROWS ONLY ");

            return builder.ToString();
        }

        internal override string GetId()
        {
            return "SELECT @@IDENTITY id";
        }
    }
}
