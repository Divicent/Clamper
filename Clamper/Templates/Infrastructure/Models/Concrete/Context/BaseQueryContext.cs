#region Usings

using Clamper.Base.Configuration.Abstract;
using Clamper.Base.Generating;
using Clamper.Tools;

#endregion

namespace Clamper.Templates.Infrastructure.Models.Concrete.Context
{
    public class BaseQueryContextTemplate : ClamperTemplate
    {
        private readonly IConfiguration _configuration;

        public BaseQueryContextTemplate(string path, IConfiguration configuration) : base(path)
        {
            _configuration = configuration;
        }

        public override string Generate()
        {
            var quote = FormatHelper.GetDbmsSpecificQuoter(_configuration);
            L($@"

//using System.Collections.Generic;
//using System.Linq;
//using {GenerationContext.BaseNamespace}.Infrastructure.Filters.Abstract;
//using {GenerationContext.BaseNamespace}.Infrastructure.Filters.Concrete;

//namespace {GenerationContext.BaseNamespace}.Infrastructure.Models.Concrete.Context
//{{
    public abstract class BaseQueryContext
    {{
        protected int? _page;
        protected int? _pageSize;
        protected int? _limit;
        protected int? _skip;
        protected int? _take;

		protected abstract bool? IsQuoted(ref string propertyName);

		protected void Sort(Queue<string> queue, string property, bool asc)
        {{
            var qotd = IsQuoted(ref property);
            if(qotd == null)
                return;
            var type = asc ? ""ASC"" : ""DESC"";
            queue.Enqueue($"" {quote("{property}")} {{type}} "");
        }}
		
		protected void ProcessFilter(Queue<string> queue, IEnumerable<IPropertyFilter> f)  
		{{
		    var filters = f?.ToList();
            if(filters == null)
                return;
            if(filters.Count < 1)
                return;

		    foreach (var propertyFilter in filters)
		    {{
		        var propName = propertyFilter.PropertyName;
                var qotd = IsQuoted(ref propName);
                switch (qotd)
                {{
                    case null:
                        continue;
                    case false:
                        var valueString = propertyFilter.Value.ToString();
                        switch (valueString)
                        {{
                            case ""true"":
                                propertyFilter.Value = ""1"";
                                break;
                            case ""false"":
                                propertyFilter.Value = ""2"";
                                break;
                        }}
                        break;
                }}

		        if (qotd == true && propertyFilter.Value.ToString() == ""true"" || propertyFilter.Value.ToString() == ""false"")

		        
                queue.Enqueue(""and"");
		        queue.Enqueue(GetExpression(propertyFilter.Type, propName, propertyFilter.Value,
		            qotd.GetValueOrDefault()));
		    }}
		}}

        private static string GetExpression(string type, string propName, object value, bool quoted )
        {{

            if (quoted && value != null)
                value = value.ToString().Replace(""'"", ""''"");

            switch (type.ToLower())
            {{
                case ""equals"":
                case ""eq"":
                    return QueryMaker.EqualsTo(propName, value, quoted);
                case ""notequals"":
                case ""neq"":
                case ""ne"":
                    return QueryMaker.NotEquals(propName, value, quoted);
                case ""contains"":
                case ""c"":
                    return QueryMaker.Contains(propName, value);
                case ""notcontains"":
                case ""doesnotcontain"":
                case ""dnc"":
                case ""nc"":
                    return QueryMaker.NotContains(propName, value);
                case ""startswith"":
                case ""sw"":
                    return QueryMaker.StartsWith(propName, value);
                case ""notstartswith"":
                case ""nsw"":
                    return QueryMaker.NotStartsWith(propName, value);
                case ""endswith"":
                case ""ew"":
                    return QueryMaker.EndsWith(propName, value);
                case ""notendswith"":
                case ""new"":
                    return QueryMaker.NotEndsWith(propName, value);
                case ""isempty"":
                case ""ie"":
                    return QueryMaker.IsEmpty(propName);
                case ""isnotempty"":
                case ""ino"":
                    return QueryMaker.IsNotEmpty(propName);
                case ""isnull"":
                case ""in"":
                    return QueryMaker.IsNull(propName);
                case ""isnotnull"":
                case ""inn"":
                    return QueryMaker.IsNotNull(propName);
                case ""greaterthan"":
                case ""gt"":
                    return QueryMaker.GreaterThan(propName, value, quoted);
                case ""lessthan"":
                case ""lt"":
                    return QueryMaker.LessThan(propName, value, quoted);
                case ""greaterthanorequals"":
                case ""gtoe"":
                case ""gte"":
                    return QueryMaker.GreaterThanOrEquals(propName, value, quoted);
                case ""lessthanorequals"":
                case ""ltoe"":
                case ""lte"":
                    return QueryMaker.LessThanOrEquals(propName, value, quoted);
                case ""istrue"":
                case ""it"":
                    return QueryMaker.IsTrue(propName);
                case ""isfalse"":
                case ""if"":
                    return QueryMaker.IsFalse(propName);
                default:
                    return """";
            }}
        }}
    }}
//}}
");

            return E();
        }
    }
}