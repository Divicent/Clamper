#region Usings

using Clamper.Base.Configuration.Abstract;
using Clamper.Base.Generating;
using Clamper.Base.Reading.Abstract;

#endregion

namespace Clamper.Templates.Infrastructure
{
    public class ProcedureContainerExtensionsTemplate : ClamperTemplate
    {
        private readonly IConfiguration _configuration;
        private readonly IDatabaseSchema _schema;

        public ProcedureContainerExtensionsTemplate(string path, IDatabaseSchema schema, IConfiguration configuration) :
            base(path)
        {
            _schema = schema;
            _configuration = configuration;
        }

        public override string Generate()
        {


            const string template =
@"
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Clamper.Infrastructure.Interfaces;
namespace {{baseNamespace}}.Infrastructure
{
    public static class ProcedureContainerExtensions
    {
{% for sp in procedures %}
		public static IEnumerable<T> {{sp.Name}}_List<T>(this IProcedureContainer pc, {{sp.ParamString}}) { return pc.QueryList<T>(""{{configuration.Schema}}.{{sp.Name}}"", new {{sp.PassString}}); }
		public static T {{sp.Name}}_Single<T>(this IProcedureContainer pc, {{sp.ParamString}}) { return pc.QuerySingle<T>(""{{configuration.Schema}}.{{sp.Name}}"", new {{sp.PassString}}); }
		public static void {{sp.Name}}_Void(this IProcedureContainer pc, {{sp.ParamString}}) { pc.Execute(""{{configuration.Schema}}.{{sp.Name}}"", new {{sp.PassString}}); }
		public async static Task<IEnumerable<T>> {{sp.Name}}_ListAsync<T>(this IProcedureContainer pc, {{sp.ParamString}}) { return await pc.QueryListAsync<T>(""{{configuration.Schema}}.{{sp.Name}}"",  new {{sp.PassString}}); }
		public async static Task<T> {{sp.Name}}_SingleAsync<T>(this IProcedureContainer pc, {{sp.ParamString}}) { return await pc.QuerySingleAsync<T>(""{{configuration.Schema}}.{{sp.Name}}"", new {{sp.PassString}}); }
		public async static Task {{sp.Name}}_VoidAsync(this IProcedureContainer pc, {{sp.ParamString}}) { await pc.ExecuteAsync(""{{configuration.Schema}}.{{sp.Name}}"", new {{sp.PassString}}); }
{% endfor %}
    }
}

";
            return Process(nameof(ProcedureContainerExtensionsTemplate),template, new
            {
                baseNamespace = GenerationContext.BaseNamespace,
                procedures = _schema.Procedures,
                configuration = _configuration
            });
        }
    }
}