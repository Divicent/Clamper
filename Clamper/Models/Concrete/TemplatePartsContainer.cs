using Clamper.Models.Abstract;

namespace Clamper.Models.Concrete
{
    public class TemplatePartsContainer : ITemplatePartsContainer
    {
        public string SqlClientNamespace { get; set; }
        public string SqlConnectionClassName { get; set; }
        public string StoredProcedureCallString { get; set; }
    }
}