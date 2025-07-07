using System.Threading.Tasks;
using UnityEngine;
using Voidwalker.Player.Ship;
using Voidwalker.Player.Ship.Upgrades;
using Voidwalker.Systems.Inkle;
using Voidwalker.Systems.Items;

namespace Voidwalker.Systems.Saving
{
    public class DataPersistenceManager
    {
        private static DataPersistenceManager instance;
        private PersistentGameData persistentGameData;
        private LevelSpecificGameData levelSpecificGameData;

        private PlayerInkStatsRuntimeSet playerStats;
        private ResourcesInventory resourcesInventory;
        private ArtifactsInventory artifactsInventory;
        private PersistentGlobalData persistentGlobalData;
        private SolarCannonUpgrades solarCannonUpgrades;
        private BroadsideCannonsUpgrades broadsideCannonsUpgrades;
        private OtherUpgrades otherUpgrades;

        private const string PLAYER_STATS_PATH = "PlayerInkStatsRuntimeSet";
        private const string RESOURCES_INVENTORY_PATH = "ResourcesInventory";
        private const string ARTIFACTS_INVENTORY_PATH = "ArtifactsInventory";
        private const string PERSISTENT_GLOBAL_DATA_PATH = "PersistentGlobalData";
        private const string SOLAR_CANNON_UPGRADES_DATA_PATH = "SolarCannonUpgrades";
        private const string BROADSIDE_CANNONS_UPGRADES_DATA_PATH = "BroadsideCannonsUpgrades";
        private const string OTHER_UPGRADES_DATA_PATH = "OtherUpgrades";

        public static DataPersistenceManager GetInstance()
        {
            instance ??= new DataPersistenceManager();
            return instance;
        }

        public void NewGame()
        {
            CreateNewPersistentGameData();
            CreateNewLevelSpecificGameData();
        }

        public void SaveGame()
        {
            SavePersistentData();
            SaveLevelSpecificData();

            string directoryPath = Application.persistentDataPath;
            Task.Run(() => DataFileHandler.SavePersistentData(persistentGameData, levelSpecificGameData, directoryPath));
        }

        public void SaveLevelIndexes(int currentLevelBuildIndex, int currentEndLevelBuildIndex, int currentLevelLoader = -1)
        {
            levelSpecificGameData.CurrentLevelBuildIndex = currentLevelBuildIndex;
            levelSpecificGameData.CurrentLevelEndSceneBuildIndex = currentEndLevelBuildIndex;

            if (currentLevelLoader >= 0)
            {
                levelSpecificGameData.CurrentLevelLoaderBuildIndex = currentLevelLoader;
            }
        }

        public void PreloadGameData()
        {
            bool hasSavedData = TryLoadData();

            if (!hasSavedData)
            {
                NewGame();
                return;
            }
            else
            {
                LoadDependencies();
            }
        }

        public bool TryLoadData()
        {
            string directoryPath = Application.persistentDataPath;
            var loadedData = DataFileHandler.LoadPersistentData(directoryPath);
            persistentGameData = loadedData.persistentData;
            levelSpecificGameData = loadedData.levelData;

            return persistentGameData != null;
        }

        public LevelSpecificGameData GetLevelSpecificData()
        {
            return levelSpecificGameData;
        }

        public void AssignData()
        {
            LoadPersistentData();

            if (levelSpecificGameData != null)
            {
                LoadLevelSpecificData();
            }
        }

        public void ResetLevelDataOnSceneChange()
        {
            levelSpecificGameData.ResetDataBetweenLevels();
        }

        private void CreateNewPersistentGameData()
        {
            LoadDependencies();
            persistentGameData = new PersistentGameData(playerStats, resourcesInventory, artifactsInventory, solarCannonUpgrades, broadsideCannonsUpgrades, otherUpgrades);
        }

        private void CreateNewLevelSpecificGameData()
        {
            levelSpecificGameData = new();
        }

        private void LoadDependencies()
        {
            playerStats = Resources.Load<PlayerInkStatsRuntimeSet>(PLAYER_STATS_PATH);
            resourcesInventory = Resources.Load<ResourcesInventory>(RESOURCES_INVENTORY_PATH);
            artifactsInventory = Resources.Load<ArtifactsInventory>(ARTIFACTS_INVENTORY_PATH);
            persistentGlobalData = Resources.Load<PersistentGlobalData>(PERSISTENT_GLOBAL_DATA_PATH);
            solarCannonUpgrades = Resources.Load<SolarCannonUpgrades>(SOLAR_CANNON_UPGRADES_DATA_PATH);
            broadsideCannonsUpgrades = Resources.Load<BroadsideCannonsUpgrades>(BROADSIDE_CANNONS_UPGRADES_DATA_PATH);
            otherUpgrades = Resources.Load<OtherUpgrades>(OTHER_UPGRADES_DATA_PATH);
        }

        private void LoadPersistentData()
        {
            DataToPlayerStats();
            DataToResourcesInventory();
            DataToArtifactsInventory();
            DataToInkGlobalVariables();
            DataToGlobalData();
            DataToUpgrades();
        }

        private void LoadLevelSpecificData()
        {
            var behaviors = Object.FindObjectsByType<ExtendedMonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            for (int i = 0; i < behaviors.Length; i++)
            {
                ExtendedMonoBehaviour behavior = behaviors[i];

                if (behavior is IPersistentData persistentSceneData)
                {
                    persistentSceneData.LoadData(levelSpecificGameData);
                }
                else if (behavior is PlayerShip player)
                {
                    player.Load(levelSpecificGameData);
                }
            }
        }

        private void SavePersistentData()
        {
            PlayerStatsToData();
            ResourcesInventoryToData();
            ArtifactsInventoryToData();
            InkGlobalVariablesToData();
            GlobalDataToData();
            UpgradesToData();
        }

        private void SaveLevelSpecificData()
        {
            var behaviors = Object.FindObjectsOfType<ExtendedMonoBehaviour>(true);

            foreach (var behavior in behaviors)
            {
                if (behavior is IPersistentData persistentSceneData)
                {
                    persistentSceneData.SaveData(ref levelSpecificGameData);
                }
            }
        }

        private void PlayerStatsToData()
        {
            persistentGameData.PlayerStatsPerception = playerStats.Perception;
            persistentGameData.PlayerStatsKnowledge = playerStats.Knowledge;
            persistentGameData.PlayerStatsResolve = playerStats.Resolve;
            persistentGameData.PlayerStatsCrewMorale = playerStats.CrewMorale;
        }

        private void DataToPlayerStats()
        {
            playerStats.Perception = persistentGameData.PlayerStatsPerception;
            playerStats.Knowledge = persistentGameData.PlayerStatsKnowledge;
            playerStats.Resolve = persistentGameData.PlayerStatsResolve;
            playerStats.CrewMorale = persistentGameData.PlayerStatsCrewMorale;
        }

        private void ResourcesInventoryToData()
        {
            foreach (var resourceItem in resourcesInventory.ItemToInventoryDataPair)
            {
                if (persistentGameData.ResourceIDResourceItemsDataPair.ContainsKey(resourceItem.Key.ResourceID))
                {
                    persistentGameData.ResourceIDResourceItemsDataPair.Remove(resourceItem.Key.ResourceID);
                }

                persistentGameData.ResourceIDResourceItemsDataPair.Add(resourceItem.Key.ResourceID, resourceItem.Value);
            }
        }

        private void DataToResourcesInventory()
        {
            foreach (var kvp in persistentGameData.ResourceIDResourceItemsDataPair)
            {
                ResourceItem resourceItem = resourcesInventory.GetItemWithID(kvp.Key);
                resourcesInventory.ItemToInventoryDataPair[resourceItem] = kvp.Value;
            }
        }

        private void ArtifactsInventoryToData()
        {
            foreach (var kvp in artifactsInventory.ArtifactToArtifactData)
            {
                if (persistentGameData.ArtifactIDArtifactRuntimeDataPair.ContainsKey(kvp.Key.ArtifactID))
                {
                    persistentGameData.ArtifactIDArtifactRuntimeDataPair.Remove(kvp.Key.ArtifactID);
                }

                persistentGameData.ArtifactIDArtifactRuntimeDataPair.Add(kvp.Key.ArtifactID, kvp.Value);
            }
        }

        private void DataToArtifactsInventory()
        {
            foreach (var kvp in persistentGameData.ArtifactIDArtifactRuntimeDataPair)
            {
                ArtifactItem artifactItem = artifactsInventory.GetArtifact(kvp.Key);
                artifactsInventory.ArtifactToArtifactData[artifactItem] = kvp.Value;
            }
        }

        private void InkGlobalVariablesToData()
        {
            var valueDictionaries = playerStats.ConvertIntInkToPrimitives();
            persistentGameData.InkGlobalVariableNameToIntPair = valueDictionaries.intDictionary;
            persistentGameData.InkGlobalVariableNameToFloatPair = valueDictionaries.floatDictionary;
            persistentGameData.InkGlobalVariableNameToStringPair = valueDictionaries.stringDictionary;
            persistentGameData.InkGlobalVariableNameToBoolPair = valueDictionaries.boolDictionary;
        }

        private void DataToInkGlobalVariables()
        {
            playerStats.ConvertPrimitivesToInk(
                persistentGameData.InkGlobalVariableNameToIntPair,
                persistentGameData.InkGlobalVariableNameToFloatPair,
                persistentGameData.InkGlobalVariableNameToStringPair,
                persistentGameData.InkGlobalVariableNameToBoolPair);
        }

        private void GlobalDataToData()
        {
            persistentGameData.SkillCheckPityCounter = persistentGlobalData.SkillDialPityCounter;
        }

        private void DataToGlobalData()
        {
            persistentGlobalData.SkillDialPityCounter = persistentGameData.SkillCheckPityCounter;
        }

        private void UpgradesToData()
        {
            persistentGameData.IsDecreaseSolarCostUpgraded = solarCannonUpgrades.DecreaseCostDescription.IsUpgraded;
            persistentGameData.IsIncreaseSolarDamageUpgraded = solarCannonUpgrades.IncreaseDamageDescription.IsUpgraded;
            persistentGameData.IsIncreaseSolarDurationUpgraded = solarCannonUpgrades.IncreaseDurationDescription.IsUpgraded;
            persistentGameData.IsDecreaseSolarCooldownUpgraded = solarCannonUpgrades.ReduceCooldownDescription.IsUpgraded;
            persistentGameData.IsIncreaseBroadsidesDamageUpgraded = broadsideCannonsUpgrades.IncreaseDamageAndWeightDescription.IsUpgraded;
            persistentGameData.IsDecreaseBroadsidesReloadUpgraded = broadsideCannonsUpgrades.DecreaseReloadSpeedDescription.IsUpgraded;
            persistentGameData.IsDecreaseBroadsidesSpreadUpdated = broadsideCannonsUpgrades.DecreaseSpreadAndIncreaseSpeedDescription.IsUpgraded;
            persistentGameData.IsIncreaseBroadsidesMaxAmmoUpgraded = broadsideCannonsUpgrades.IncreaseMaxAmmoCapacityDescription.IsUpgraded;
            persistentGameData.IsIncreaseMaxEnergyUpgraded = otherUpgrades.IncreaseEnergyCapacityDescription.IsUpgraded;
            persistentGameData.IsDecreaseCollisionDamageUpgraded = otherUpgrades.DecreaseCollisionDamageDescription.IsUpgraded;
            persistentGameData.IsIncreaseHPUpgraded = otherUpgrades.IncreaseShipHealthDescription.IsUpgraded;
            persistentGameData.IsDecreaseShieldCostUpgraded = otherUpgrades.DecreaseShieldCostDescription.IsUpgraded;
        }

        private void DataToUpgrades()
        {
            solarCannonUpgrades.DecreaseCostDescription.IsUpgraded = persistentGameData.IsDecreaseSolarCostUpgraded;
            solarCannonUpgrades.IncreaseDamageDescription.IsUpgraded = persistentGameData.IsIncreaseSolarDamageUpgraded;
            solarCannonUpgrades.IncreaseDurationDescription.IsUpgraded = persistentGameData.IsIncreaseSolarDurationUpgraded;
            solarCannonUpgrades.ReduceCooldownDescription.IsUpgraded = persistentGameData.IsDecreaseSolarCooldownUpgraded;
            broadsideCannonsUpgrades.IncreaseDamageAndWeightDescription.IsUpgraded = persistentGameData.IsIncreaseBroadsidesDamageUpgraded;
            broadsideCannonsUpgrades.DecreaseReloadSpeedDescription.IsUpgraded = persistentGameData.IsDecreaseBroadsidesReloadUpgraded;
            broadsideCannonsUpgrades.DecreaseSpreadAndIncreaseSpeedDescription.IsUpgraded = persistentGameData.IsDecreaseBroadsidesSpreadUpdated;
            broadsideCannonsUpgrades.IncreaseMaxAmmoCapacityDescription.IsUpgraded = persistentGameData.IsIncreaseBroadsidesMaxAmmoUpgraded;
            otherUpgrades.IncreaseEnergyCapacityDescription.IsUpgraded = persistentGameData.IsIncreaseMaxEnergyUpgraded;
            otherUpgrades.DecreaseCollisionDamageDescription.IsUpgraded = persistentGameData.IsDecreaseCollisionDamageUpgraded;
            otherUpgrades.IncreaseShipHealthDescription.IsUpgraded = persistentGameData.IsIncreaseHPUpgraded;
            otherUpgrades.DecreaseShieldCostDescription.IsUpgraded = persistentGameData.IsDecreaseShieldCostUpgraded;
        }
    }
}
