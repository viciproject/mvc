using Vici.Core.Parser;

namespace Vici.Mvc
{
    public interface ITemplateContext : IParserContext
    {
        Template Template { get; }
        View View { get; }
    }
}