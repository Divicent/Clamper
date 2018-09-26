#region Usings

using DotLiquid;
using Clamper.Models.Abstract;

#endregion

namespace Clamper.Models.Concrete
{
    public class EnumValue : IEnumValue, ILiquidizable
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public string FieldName { get; set; }
        public object ToLiquid()
        {
            return new
            {
                Name,
                Value,
                FieldName
            };
        }
    }
}