#region Usings

using Clamper.Base.Configuration.Abstract;
using Clamper.Models.Abstract;
using Clamper.Tools;

#endregion

namespace Clamper.Templates.Infrastructure.Models.Concrete
{
    public class ViewTemplate : ClamperTemplate
    {
        private readonly IConfiguration _configuration;
        private readonly IView _view;

        public ViewTemplate(string path, IView view, IConfiguration configuration) : base(path)
        {
            _view = view;
            _configuration = configuration;
        }

        public override string Generate()
        {

            const string template =
                @"
                [Table(""{{quotedSchema}}.{{quotedName}}"")]
                public class {{name}} {{absImplement}}
                {

{% if abstractModelsEnabled %}
                    public {{name}}() { }
        
                    public {{name}}(I{{name}} model) 
                    {
                        if(model == null) { return; }
{% for attribute in attributes %}
                        {{attribute.Name}} = model.{{attribute.Name}};
{% endfor %}
                    }

{% endif %}

{% for atd in attributes %}
{% if atd.HasComment %}
                    /// <summary>
                    /// {atd.Comment}
                    /// </summary>
{% endif %}
                    public {{atd.DataType}} {{atd.Name}} { get; set; }
{% endfor %}
                }
";

            var quote = FormatHelper.GetDbmsSpecificQuoter(_configuration);


            return Process(nameof(ViewTemplate), template, new
            {
                name = _view.Name,
                quotedSchema = quote(_view.Schema),
                quotedName = quote(_view.Name),
                abstractModelsEnabled = _configuration.AbstractModelsEnabled,
                absImplement = _configuration.AbstractModelsEnabled ? $": I{_view.Name}" : "",
                attributes = _view.Attributes
            });
        }
    }
}