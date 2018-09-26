#region Usings

using System.Collections.Generic;
using DotLiquid;
using Clamper.Models.Abstract;

#endregion

namespace Clamper.Models.Concrete
{
    public class StoredProcedure : IStoredProcedure, ILiquidizable
    {
        public string Name { get; set; }
        public string PassString { get; set; }
        public string ParamString { get; set; }
        public List<ProcedureParameter> Parameters { get; set; }
        public object ToLiquid()
        {
            return new
            {
                Name,
                ParamString,
                PassString,
                Parameters
            };
        }
    }
}