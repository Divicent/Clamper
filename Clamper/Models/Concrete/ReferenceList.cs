#region Usings

using DotLiquid;
using Clamper.Extensions;
using Clamper.Models.Abstract;

#endregion

namespace Clamper.Models.Concrete
{
    public class ReferenceList : IReferenceList, ILiquidizable
    {
        public string ReferencedRelationName { get; set; }
        public string ReferencedPropertyName { get; set; }
        public string ReferencedPropertyOnThisRelation { get; set; }
        public object ToLiquid()
        {
            return new
            {
                ReferencedRelationName,
                ReferencedPropertyName,
                ReferencedPropertyOnThisRelation,
                ReferencedRelationNamePlural = ReferencedRelationName.ToPlural()
            };
        }
    }
}