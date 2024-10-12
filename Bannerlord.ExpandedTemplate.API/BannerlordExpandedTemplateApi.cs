using Bannerlord.ExpandedTemplate.API.Logging;
using Bannerlord.ExpandedTemplate.Integration;

namespace Bannerlord.ExpandedTemplate.API;

public class BannerlordExpandedTemplateApi
{
    private ExpandedTemplateSubModule _subModule = new ();

    public BannerlordExpandedTemplateApi UseLoggerFactory(ILoggerFactory loggerFactory)
    {
        _subModule = new ExpandedTemplateSubModule(new LoggerFactoryAdapter(loggerFactory));
        return this;
    }

    public void Bind()
    {
        _subModule.Inject();
    }
}