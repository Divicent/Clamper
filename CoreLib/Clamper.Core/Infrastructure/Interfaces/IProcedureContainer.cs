using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Clamper.Core.Infrastructure.Interfaces
{
    /// <summary>
    /// Contains methods for invoke all stored procedures in the data store.
    /// these methods will directly invoke without any side effects. all parameters are optional in all methods.
    /// 
    /// for each stored procedure there are 3 methods, because the generation process cannot identify the result at the time the code is generated.
    /// void methods are for execute procedures without expecting a result which are suffixed as _Void.
    /// list methods are to execute procedures and expect a collection which are suffixed with as _List
    /// single methods are to execute a procedures and expect a single object. object can be any type these methods are suffixed as _Single
    /// </summary>
    public interface IProcedureContainer
    {
        void Execute(string name, object parameters);

        T QuerySingle<T>(string name, object parameters);

        IEnumerable<T> QueryList<T>(string name, object parameters);

        Task ExecuteAsync(string name, object parameters);

        Task<T> QuerySingleAsync<T>(string name, object parameters);

        Task<IEnumerable<T>> QueryListAsync<T>(string name, object parameters);
    }
}

