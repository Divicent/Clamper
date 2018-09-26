using System.Collections.Generic;
using System.Data;
using Clamper.Core.Infrastructure.Filters.Concrete;

namespace Clamper.Core.Infrastructure.Filters.Abstract
{
    public interface IRepoQuery
    {
        string Target { get; set; }
        Queue<FilterExpression> Where { get; set; }
        Queue<OrderExpression> Order { get; set; }
        int? PageSize { get; set; }
        int? Page { get; set; }
        int? Limit { get; set; }
        int? Skip { get; set; }
        int? Take { get; set; }
        IDbTransaction Transaction { get; set; }
        IEnumerable<string> Columns { get; set; }
    }
}


