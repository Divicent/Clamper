#region Usings

using Clamper.Templates.Abstract;

#endregion

namespace Clamper.Base.Generating
{
    internal class TemplateFile
    {
        internal string Path { get; set; }
        internal ITemplate Template { get; set; }

        internal string Generate()
        {
            return Template.Generate();
        }
    }
}