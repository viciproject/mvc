using Vici.Core.Parser;

namespace Vici.Mvc
{
    public interface ITemplateParser
    {
        ICompiledTemplate Parse(string text);

        string Render(ICompiledTemplate compiledTemplate, IParserContext parserContext);
    }
}