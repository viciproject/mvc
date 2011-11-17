namespace Vici.Mvc
{
    public interface IViewEngine
    {
        ITemplateParser Parser { get; }
        bool CanParse(string fileName);
    }
}