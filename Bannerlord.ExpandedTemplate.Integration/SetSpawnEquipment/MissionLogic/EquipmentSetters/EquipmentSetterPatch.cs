using Bannerlord.ExpandedTemplate.Domain.EquipmentPool;
using Bannerlord.ExpandedTemplate.Domain.Logging.Port;
using Bannerlord.ExpandedTemplate.Integration.SetSpawnEquipment.Mappers;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using Equipment = TaleWorlds.Core.Equipment;

namespace Bannerlord.ExpandedTemplate.Integration.SetSpawnEquipment.MissionLogic.EquipmentSetters;

[HarmonyPatch(typeof(Mission))]
[HarmonyPatch("SpawnAgent")]
public class EquipmentSetterPatch
{
    private static HeroEquipmentGetter _heroEquipmentGetter;
    private static IGetEquipmentPool _getEquipmentPool;
    private static CharacterEquipmentRosterReference _characterEquipmentRosterReference;
    private static EquipmentPoolsMapper _equipmentPoolsMapper;
    private static ILogger _logger;

    public static bool Prefix(AgentBuildData agentBuildData)
    {
        _logger.Debug($"--- START Equipment for Agent {agentBuildData.AgentCharacter.StringId} ---");
        
        MBEquipmentRoster? equipmentRosterReference =
            _characterEquipmentRosterReference.GetEquipmentRoster(agentBuildData.AgentCharacter);

        if (equipmentRosterReference is null) return true;
        if (!CanOverrideEquipment(agentBuildData.AgentCharacter))
        {
            _logger.Debug($"{agentBuildData.AgentCharacter.StringId} is not applicable to equipment override");
            return true;
        }

        Domain.EquipmentPool.Model.EquipmentPool equipmentPool = GetEquipmentPool(agentBuildData.AgentCharacter);

        _logger.Debug(
            $"Selecting equipment pool number '{equipmentPool.GetPoolId()}' which contains {equipmentPool.GetEquipmentLoadouts().Count} loadouts.");
        
        Equipment equipment;
        if (agentBuildData.AgentCharacter.IsHero)
        {
            equipment = _heroEquipmentGetter.GetEquipmentFromEquipmentPool(agentBuildData.AgentCharacter,
                equipmentPool);
        }
        else
        {
            MBEquipmentRoster mbEquipmentRoster =
                _equipmentPoolsMapper.MapEquipmentPool(equipmentPool, equipmentRosterReference);
            var characterEquipmentContainer = new BasicCharacterObject();
            _characterEquipmentRosterReference.SetEquipmentRoster(characterEquipmentContainer, mbEquipmentRoster);

            equipment = Equipment.GetRandomEquipmentElements(characterEquipmentContainer,
                !Game.Current.GameType.IsCoreOnlyGameMode,
                agentBuildData.AgentCivilianEquipment, agentBuildData.AgentEquipmentSeed);
        }

        agentBuildData.FixedEquipment(true).Equipment(equipment);

        if (equipment.IsEmpty())
            _logger.Warn(
                $"Troop '{agentBuildData.AgentCharacter.Name.Value}' with id '{agentBuildData.AgentCharacter.StringId}' spawned with no equipment.");

        _logger.Debug($"--- END Equipment for Agent {agentBuildData.AgentCharacter.StringId} ---");

        return true;
    }

    /// <summary>
    ///     Temporary initialiser
    /// </summary>
    /// <param name="heroEquipmentGetter"></param>
    /// <param name="getEquipmentPool"></param>
    /// <param name="characterEquipmentRosterReference"></param>
    /// <param name="equipmentPoolsMapper"></param>
    public static void Initialise(HeroEquipmentGetter heroEquipmentGetter,
        IGetEquipmentPool getEquipmentPool,
        CharacterEquipmentRosterReference characterEquipmentRosterReference,
        EquipmentPoolsMapper equipmentPoolsMapper, ILoggerFactory loggerFactory)
    {
        _heroEquipmentGetter = heroEquipmentGetter;
        _getEquipmentPool = getEquipmentPool;
        _characterEquipmentRosterReference = characterEquipmentRosterReference;
        _equipmentPoolsMapper = equipmentPoolsMapper;
        _logger = loggerFactory.CreateLogger<EquipmentSetterPatch>();
    }

    private static Domain.EquipmentPool.Model.EquipmentPool GetEquipmentPool(BasicCharacterObject character)
    {
        string id = character.StringId;
        if (character is CharacterObject characterObject)
            id = characterObject.OriginalCharacter?.StringId ?? id;

        var equipmentPool = _getEquipmentPool.GetTroopEquipmentPool(id);
        if (equipmentPool.IsEmpty())
            equipmentPool =
                _getEquipmentPool.GetTroopEquipmentPool(_characterEquipmentRosterReference.GetEquipmentRoster(character)
                    .StringId);

        return equipmentPool;
    }

    private static bool CanOverrideEquipment(BasicCharacterObject character)
    {
        return character is not null &&
               !(Clan.PlayerClan?.Heroes?.Exists(hero =>
                   hero?.StringId is not null && character.StringId == hero.StringId) ?? true);
    }
}