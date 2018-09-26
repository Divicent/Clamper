#region Usings

using DotLiquid;
using Clamper.Models.Abstract;

#endregion

namespace Clamper.Models.Concrete
{
    public class ForeignKeyAttribute : IForeignKeyAttribute, ILiquidizable
    {
        public IAttribute ReferencingNonForeignKeyAttribute { get; set; }

        public string ReferencingRelationName { get; set; }

        public string ReferencingTableColumnName { get; set; }

        public object ToLiquid()
        {
            return new
            {
                ReferencingNonForeignKeyAttribute,
                ReferencingRelationName,
                ReferencingTableColumnName,
                Fix = ReferencingNonForeignKeyAttribute.DataType.EndsWith("?") ? ".GetValueOrDefault()" : ""
            };
        }
    }
}