#region Usings

using System.Collections.Generic;
using DotLiquid;
using Clamper.Models.Abstract;

#endregion

namespace Clamper.Models.Concrete
{
    public class Enum : IEnum, ILiquidizable
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public List<IEnumValue> Values { get; set; }
        public object ToLiquid()
        {
            return new
            {
                Type,
                Name,
                Values
            };
        }
    }
}