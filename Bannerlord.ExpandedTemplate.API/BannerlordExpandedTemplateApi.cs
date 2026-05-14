using Bannerlord.ExpandedTemplate.API.Logging;
using Bannerlord.ExpandedTemplate.Integration;

namespace Bannerlord.ExpandedTemplate.API;

public class BannerlordExpandedTemplateApi
{
    private ExpandedTemplateSubModule? _expandedTemplateSubModule;

    public BannerlordExpandedTemplateApi UseLoggerFactory(ILoggerFactory loggerFactory)
    {
        _expandedTemplateSubModule = new ExpandedTemplateSubModule(new LoggerFactoryAdapter(loggerFactory));
        return this;
    }

    public void Bind()
    {
        (_expandedTemplateSubModule ??= new ExpandedTemplateSubModule()).Inject();
    }
}