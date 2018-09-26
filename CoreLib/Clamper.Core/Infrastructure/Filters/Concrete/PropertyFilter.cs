using Clamper.Core.Infrastructure.Filters.Abstract;

namespace Clamper.Core.Infrastructure.Filters.Concrete
{
    internal class PropertyFilter : IPropertyFilter
    {
        public string PropertyName { get; set; }
        public string Type { get; set; }
        public object Value { get; set; }
    }
}

