using Bannerlord.ExpandedTemplate.Domain.EquipmentPool;
using Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Port;
using Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Util;
using Bannerlord.ExpandedTemplate.Domain.Logging.Port;
using Bannerlord.ExpandedTemplate.Infrastructure.Caching;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.Get.Battle;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.Get.Civilian;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.Get.Siege;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Mappers;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentPool;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentRosters;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentRosters.Battle;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentRosters.Civilian;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentRosters.Pool;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentRosters.Siege;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Repositories;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Xml;
using Bannerlord.ExpandedTemplate.Integration.EquipmentPool;
using Bannerlord.ExpandedTemplate.Integration.EquipmentPool.List.Repositories.Spi;
using Bannerlord.ExpandedTemplate.Integration.EquipmentPool.Spi;
using Bannerlord.ExpandedTemplate.Integration.SetSpawnEquipment.Mappers;
using Bannerlord.ExpandedTemplate.Integration.SetSpawnEquipment.MissionLogic.EquipmentSetters;
using Harmony.DependencyInjection.Patches;
using Microsoft.Extensions.DependencyInjection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using Random = Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Util.Random;

namespace Bannerlord.ExpandedTemplate.Integration.Module
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection ConfigureServices(IServiceCollection services)
        {
            // Register logging services
            services.AddLogging();
            
            // Register caching services
            services.AddSingleton<ICachingProvider, InMemoryCacheProvider>();
            services.AddSingleton<ICacheInvalidator>(sp => (ICacheInvalidator)sp.GetRequiredService<ICachingProvider>());

            // Register XML processing and repositories
            services.AddSingleton<IXmlProcessor, MergedModulesXmlProcessor>();
            services.AddSingleton<INpcCharacterRepository, NpcCharacterRepository>();
            services.AddSingleton<IEquipmentRosterRepository, EquipmentRosterRepository>();
            services.AddSingleton<IEquipmentRostersReader, EquipmentRostersReader>();
            services.AddSingleton<IEquipmentRosterXmlReader, EquipmentRosterXmlReader>();
            services.AddSingleton<IEquipmentSetXmlReader, EquipmentSetXmlReader>();
            
            // Register mappers
            services.AddSingleton<IEquipmentSetMapper, EquipmentSetMapper>();
            services.AddSingleton<IEquipmentRosterMapper, EquipmentRosterMapper>();
            services.AddSingleton<INpcCharacterMapper, NpcCharacterMapper>();
            
            services.AddSingleton<EquipmentPoolsMapper>();
            
            // Register equipment provider services
            services.AddSingleton<INpcCharacterWithResolvedEquipmentProvider, NpcCharacterWithResolvedEquipmentProvider>();
            services.AddSingleton<IPoolEquipmentRosterProvider, PoolEquipmentRosterProvider>();
            
            // Register equipment roster providers
            services.AddSingleton<SiegeEquipmentRosterProvider>();
            services.AddSingleton<CivilianEquipmentRosterProvider>();
            services.AddSingleton<BattleEquipmentRosterProvider>(sp =>
                new BattleEquipmentRosterProvider(
                    sp.GetRequiredService<SiegeEquipmentRosterProvider>(),
                    sp.GetRequiredService<CivilianEquipmentRosterProvider>(),
                    sp.GetRequiredService<INpcCharacterWithResolvedEquipmentProvider>()));
            
            // Register equipment provider services - each wired to its own EquipmentPoolsProvider
            // to avoid DI ambiguity (GetRequiredService<IEquipmentPoolsProvider> would return the last registration)
            services.AddSingleton<ITroopBattleEquipmentProvider>(sp =>
                new TroopBattleEquipmentPoolProvider(
                    sp.GetRequiredService<ILoggerFactory>(),
                    new EquipmentPoolsProvider(
                        sp.GetRequiredService<BattleEquipmentRosterProvider>(),
                        sp.GetRequiredService<IPoolEquipmentRosterProvider>(),
                        sp.GetRequiredService<IEquipmentRosterMapper>(),
                        sp.GetRequiredService<ICachingProvider>())));

            services.AddSingleton<ITroopSiegeEquipmentProvider>(sp =>
                new TroopSiegeEquipmentPoolProvider(
                    sp.GetRequiredService<ILoggerFactory>(),
                    new EquipmentPoolsProvider(
                        sp.GetRequiredService<SiegeEquipmentRosterProvider>(),
                        sp.GetRequiredService<IPoolEquipmentRosterProvider>(),
                        sp.GetRequiredService<IEquipmentRosterMapper>(),
                        sp.GetRequiredService<ICachingProvider>())));

            services.AddSingleton<ITroopCivilianEquipmentProvider>(sp =>
                new TroopCivilianEquipmentPoolProvider(
                    sp.GetRequiredService<ILoggerFactory>(),
                    new EquipmentPoolsProvider(
                        sp.GetRequiredService<CivilianEquipmentRosterProvider>(),
                        sp.GetRequiredService<IPoolEquipmentRosterProvider>(),
                        sp.GetRequiredService<IEquipmentRosterMapper>(),
                        sp.GetRequiredService<ICachingProvider>())));
            services.AddSingleton<IEncounterTypeProvider, EncounterTypeProvider>();
            
            // Register domain services
            services.AddSingleton<IRandom, Random>();
            services.AddSingleton<IEquipmentPoolPicker, EquipmentPoolPicker>();
            services.AddSingleton<IGetEquipmentPool, GetEquipmentPool>();
            services.AddSingleton<IGetEquipmentPoolsUtil, GetEquipmentPoolsUtil>();
            services.AddSingleton<IGetEquipment, GetEquipment>();
            services.AddSingleton<IEquipmentComparison, EquipmentComparison>();
            
            // Register mission-specific services
            services.AddTransient<CharacterEquipmentRosterReference>();
            services.AddTransient<HeroEquipmentGetter>();
            services.AddSingleton<BannerlordEquipmentMapper>();
            
            // Register campaign behaviors
            services.AddTransient<CampaignBehaviorBase>(sp =>
                new CampaignLoadEquipmentPoolHandler(
                    sp.GetRequiredService<ICacheInvalidator>(),
                    new EquipmentPoolsProvider(
                        sp.GetRequiredService<BattleEquipmentRosterProvider>(),
                        sp.GetRequiredService<IPoolEquipmentRosterProvider>(),
                        sp.GetRequiredService<IEquipmentRosterMapper>(),
                        sp.GetRequiredService<ICachingProvider>()),
                    new EquipmentPoolsProvider(
                        sp.GetRequiredService<CivilianEquipmentRosterProvider>(),
                        sp.GetRequiredService<IPoolEquipmentRosterProvider>(),
                        sp.GetRequiredService<IEquipmentRosterMapper>(),
                        sp.GetRequiredService<ICachingProvider>()),
                    new EquipmentPoolsProvider(
                        sp.GetRequiredService<SiegeEquipmentRosterProvider>(),
                        sp.GetRequiredService<IPoolEquipmentRosterProvider>(),
                        sp.GetRequiredService<IEquipmentRosterMapper>(),
                        sp.GetRequiredService<ICachingProvider>())));

            services.AddTransient<EquipmentFactory>();
            services.AddSingleton<IPatch, EquipmentSetterPatch>();
            
            return services;
        }
        
        public static void RegisterGameDependencies(IServiceCollection services)
        {
            // Register game-specific services that are only available at runtime
            if (Game.Current?.ObjectManager != null)
            {
                services.AddSingleton(Game.Current.ObjectManager);
                
                // Now we can register EquipmentMapper since MBObjectManager is available
                services.AddSingleton<EquipmentMapper>();
            }
        }
    }
}