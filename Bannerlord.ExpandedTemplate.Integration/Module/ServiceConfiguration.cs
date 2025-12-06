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
using Bannerlord.ExpandedTemplate.Infrastructure.Logging;
using Bannerlord.ExpandedTemplate.Integration.EquipmentPool;
using Bannerlord.ExpandedTemplate.Integration.EquipmentPool.List.Repositories.Spi;
using Bannerlord.ExpandedTemplate.Integration.EquipmentPool.Spi;
using Bannerlord.ExpandedTemplate.Integration.SetSpawnEquipment.Mappers;
using Bannerlord.ExpandedTemplate.Integration.SetSpawnEquipment.MissionLogic;
using Bannerlord.ExpandedTemplate.Integration.SetSpawnEquipment.MissionLogic.EquipmentSetters;
using Microsoft.Extensions.DependencyInjection;
using TaleWorlds.ObjectSystem;
using Random = Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Util.Random;

namespace Bannerlord.ExpandedTemplate.Integration.Module
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection ConfigureServices(IServiceCollection services)
        {
            // Register logging services
            services.AddSingleton<ILoggerFactory, ConsoleLoggerFactory>();
            
            // Register caching services
            services.AddSingleton<ICachingProvider, InMemoryCacheProvider>();
            services.AddSingleton<ICacheInvalidator>(sp => (ICacheInvalidator)sp.GetRequiredService<ICachingProvider>());

            // Register XML processing and repositories
            services.AddSingleton<IXmlProcessor, MergedModulesXmlProcessor>();
            services.AddSingleton<INpcCharacterRepository, NpcCharacterRepository>();
            services.AddSingleton<IEquipmentRosterRepository, EquipmentRosterRepository>();
            
            // Register mappers
            services.AddSingleton<IEquipmentSetMapper, EquipmentSetMapper>();
            services.AddSingleton<IEquipmentRosterMapper, EquipmentRosterMapper>();
            services.AddSingleton<INpcCharacterMapper, NpcCharacterMapper>();
            
            // We can't register EquipmentMapper here because it depends on MBObjectManager
            // which is only available at runtime
            
            services.AddSingleton<EquipmentPoolsMapper>();
            
            // Register equipment provider services
            services.AddSingleton<INpcCharacterWithResolvedEquipmentProvider, NpcCharacterWithResolvedEquipmentProvider>();
            services.AddSingleton<IPoolEquipmentRosterProvider, PoolEquipmentRosterProvider>();
            
            // Register equipment roster providers
            services.AddSingleton<SiegeEquipmentRosterProvider>();
            services.AddSingleton<CivilianEquipmentRosterProvider>();
            services.AddSingleton<BattleEquipmentRosterProvider>();
            
            // Register equipment pools providers
            services.AddSingleton<IEquipmentPoolsProvider, EquipmentPoolsProvider>(sp => 
                new EquipmentPoolsProvider(
                    sp.GetRequiredService<BattleEquipmentRosterProvider>(),
                    sp.GetRequiredService<IPoolEquipmentRosterProvider>(),
                    sp.GetRequiredService<IEquipmentRosterMapper>(),
                    sp.GetRequiredService<ICachingProvider>()
                ));
            
            services.AddSingleton<IEquipmentPoolsProvider, EquipmentPoolsProvider>(sp => 
                new EquipmentPoolsProvider(
                    sp.GetRequiredService<SiegeEquipmentRosterProvider>(),
                    sp.GetRequiredService<IPoolEquipmentRosterProvider>(),
                    sp.GetRequiredService<IEquipmentRosterMapper>(),
                    sp.GetRequiredService<ICachingProvider>()
                ));
            
            services.AddSingleton<IEquipmentPoolsProvider, EquipmentPoolsProvider>(sp => 
                new EquipmentPoolsProvider(
                    sp.GetRequiredService<CivilianEquipmentRosterProvider>(),
                    sp.GetRequiredService<IPoolEquipmentRosterProvider>(),
                    sp.GetRequiredService<IEquipmentRosterMapper>(),
                    sp.GetRequiredService<ICachingProvider>()
                ));
            
            // Register equipment provider services
            services.AddSingleton<ITroopBattleEquipmentProvider, TroopBattleEquipmentPoolProvider>();
            services.AddSingleton<ITroopSiegeEquipmentProvider, TroopSiegeEquipmentPoolProvider>();
            services.AddSingleton<ITroopCivilianEquipmentProvider, TroopCivilianEquipmentPoolProvider>();
            services.AddSingleton<IEncounterTypeProvider, EncounterTypeProvider>();
            
            // Register domain services
            services.AddSingleton<IRandom, Random>();
            services.AddSingleton<IEquipmentPoolPicker, EquipmentPoolPicker>();
            services.AddSingleton<IGetEquipmentPool, GetEquipmentPool>();
            services.AddSingleton<IGetEquipment, GetEquipment>();
            
            // Register mission-specific services
            services.AddTransient<CharacterEquipmentRosterReference>();
            services.AddTransient<HeroEquipmentSetter>();
            services.AddTransient<TroopEquipmentPoolSetter>();
            services.AddTransient<EquipmentSetterMissionLogic>();
            
            // Register campaign behaviors
            services.AddTransient<CampaignLoadEquipmentPoolHandler>();
            
            // Register injector with service provider
            services.AddSingleton<SubModuleInjector>(sp => new SubModuleInjector(
                sp.GetRequiredService<ILoggerFactory>(),
                sp
            ));
            
            return services;
        }
        
        public static void RegisterGameDependencies(IServiceCollection services)
        {
            // Register game-specific services that are only available at runtime
            if (TaleWorlds.Core.Game.Current?.ObjectManager != null)
            {
                services.AddSingleton(TaleWorlds.Core.Game.Current.ObjectManager);
                
                // Now we can register EquipmentMapper since MBObjectManager is available
                services.AddSingleton<EquipmentMapper>();
            }
        }
    }
}