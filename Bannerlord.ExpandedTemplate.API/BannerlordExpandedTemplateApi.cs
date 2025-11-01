using Bannerlord.ExpandedTemplate.API.Logging;
using Bannerlord.ExpandedTemplate.Integration;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace Bannerlord.ExpandedTemplate.API;

public class BannerlordExpandedTemplateApi
{
    private ExpandedTemplateBootstrapper _expandedTemplateBootstrapper = new();

    public BannerlordExpandedTemplateApi UseLoggerFactory(ILoggerFactory loggerFactory)
    {
        _expandedTemplateBootstrapper = new ExpandedTemplateBootstrapper(new LoggerFactoryAdapter(loggerFactory));
        return this;
    }

    public void OnBeforeMissionBehaviorInitialize(Mission mission)
    {
        _expandedTemplateBootstrapper.InitializeMission(mission);
    }

    public void InitializeGameStarter(Game game, IGameStarter starterObject)
    {
        _expandedTemplateBootstrapper.InitializeCampaign(game, starterObject);
    }
}