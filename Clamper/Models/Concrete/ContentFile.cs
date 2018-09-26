#region Usings

using Clamper.Models.Abstract;

#endregion

namespace Clamper.Models.Concrete
{
    internal class ContentFile : IContentFile
    {
        public string Content { get; set; }
        public string Path { get; set; }
        public bool External { get; set; }
    }
}