using System;
using System.Linq;
using System.Text.RegularExpressions;
using AdvancedBuildingManager.Components;
using Colossal.Entities;
using Colossal.Serialization.Entities;
using Game;
using Game.Common;
using Game.Economy;
using Game.Prefabs;
using Game.Simulation;
using Unity.Collections;
using Unity.Entities;

namespace AdvancedBuildingManager.Systems
{
    public partial class LevelSystem : GameSystemBase
    {
        public string prefix = "StarQ ABM ";

        public void SetLevel(
            EntityManager entityManager,
            PrefabSystem prefabSystem,
            Entity entity,
            Entity prefab,
            int level
        )
        {
            try
            {
                if (
                    entityManager.TryGetComponent(
                        prefab,
                        out SpawnableBuildingData spawnableBuildingData
                    ) && entityManager.TryGetComponent(prefab, out ConsumptionData consumptionData)
                )
                {
                    //spawnableBuildingData.m_ZonePrefab = DataRetriever
                    //    .zoneDataInfos[new Random().Next(0, DataRetriever.zoneDataInfos.Count)]
                    //    .Entity;

                    entityManager.TryGetComponent(
                        prefab,
                        out Game.Prefabs.BuildingData buildingData
                    );

                    var match = DataRetriever.zoneDataInfos.FirstOrDefault(b =>
                        b.PrefabName
                        == prefabSystem.GetPrefabName(spawnableBuildingData.m_ZonePrefab)
                    );

                    entityManager.TryGetComponent(
                        prefab,
                        out Game.Prefabs.BuildingPropertyData buildingPropertyData
                    );
                    bool isStorage = buildingPropertyData.m_AllowedStored > Resource.NoResource;

                    EconomyParameterData economyParameterData =
                        SystemAPI.GetSingleton<EconomyParameterData>();
                    int newUpkeep = PropertyRenterSystem.GetUpkeep(
                        level,
                        match.Upkeep,
                        buildingData.m_LotSize.x * buildingData.m_LotSize.y,
                        match.AreaType,
                        ref economyParameterData,
                        isStorage
                    );

                    consumptionData.m_Upkeep = newUpkeep;
                    spawnableBuildingData.m_Level = (byte)level;

                    entityManager.AddComponentData(prefab, spawnableBuildingData);
                    entityManager.AddComponentData(prefab, consumptionData);
                    entityManager.AddComponent<Updated>(prefab);
                    entityManager.AddComponent<Updated>(entity);
                }
            }
            catch (Exception ex)
            {
                Mod.log.Info(ex.ToString());
            }
        }

        public void CreateVariants(
            EntityManager entityManager,
            PrefabSystem prefabSystem,
            Entity entity,
            PrefabBase currentPrefabBase
        )
        {
            string ogName =
                $"{Regex.Replace(currentPrefabBase.name.Replace(prefix, ""), @"\s\d{9}$", "")}";
            // ogName in case the mod already replaced the PrefabRef once
            string vName = $"{prefix}{ogName}";

            //prefabSystem.TryGetPrefab(
            //    new PrefabID("BuildingPrefab", ogName),
            //    out PrefabBase ogPrefabBase
            //);

            //Entity ogPrefabEntity = prefabSystem.GetEntity(ogPrefabBase);

            int ver = 1;
            string newName = $"{vName} {ver:D9}";
            while (prefabSystem.TryGetPrefab(new PrefabID("BuildingPrefab", newName), out _))
            {
                ver++;
                newName = $"{vName} {ver:D9}";
            }
            BuildingVariants variants = new() { Name = newName, OGName = ogName };

            PrefabBase newPrefabBase = prefabSystem.DuplicatePrefab(currentPrefabBase, newName);
            Entity newEntity = prefabSystem.GetEntity(newPrefabBase);

            entityManager.AddComponent<Updated>(newEntity);
            entityManager.AddComponent<Updated>(entity);
            entityManager.AddComponentData(entity, variants);
            entityManager.AddComponentData(entity, new PrefabRef() { m_Prefab = newEntity });

            Mod.log.Info($"Created {newName} from {ogName}");

            //TODO: Store the ogPrefab info in a component and reload them on game load, also save to disk.
        }

        protected override void OnUpdate() { }

        public string UnPrefixify(string og)
        {
            string withoutPrefix = og.Replace(prefix, "").TrimStart();

            int lastSpaceIndex = withoutPrefix.LastIndexOf(' ');
            if (lastSpaceIndex == -1)
                return withoutPrefix;

            string namePart = withoutPrefix[..lastSpaceIndex];
            string numberPart = withoutPrefix[(lastSpaceIndex + 1)..];

            if (ulong.TryParse(numberPart, out ulong number))
            {
                return $"{namePart} [Variant: {number}]";
            }

            return withoutPrefix;
        }
    }
}
