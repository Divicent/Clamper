using System.Collections.Generic;
using System.Data;
using Clamper.Core.Infrastructure.Filters.Abstract;

namespace Clamper.Core.Infrastructure.Filters.Concrete
{
    public class RepoQuery : IRepoQuery
    {
        public RepoQuery()
        {
        }

        public string Target { get; set; }
        public Queue<FilterExpression> Where { get; set; }
        public Queue<OrderExpression> Order { get; set; }
        public int? PageSize { get; set; }
        public int? Page { get; set; }
        public int? Limit { get; set; }
        public int? Skip { get; set; }
        public int? Take { get; set; }
        public IDbTransaction Transaction { get; set; }
        public IEnumerable<string> Columns { get; set; }
    }

}

