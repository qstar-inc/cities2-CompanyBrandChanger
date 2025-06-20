using System;
using System.Linq;
using System.Text.RegularExpressions;
using Colossal.Entities;
using Colossal.IO.AssetDatabase;
using Colossal.Json;
using Game;
using Game.Common;
using Game.Economy;
using Game.Prefabs;
using Game.Simulation;
using Game.Triggers;
using Game.Zones;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

namespace AdvancedBuildingManager.Systems
{
    public partial class LevelSystem : GameSystemBase
    {
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
                //Mod.log.Info("Trying SetLevel");
                //if (entityManager.TryGetComponent(prefab, out BuildingPropertyData propertyData))
                //{
                //    entityManager.TryGetComponent(
                //            prefab,
                //            out SpawnableBuildingData spawnableBuildingData);
                //    Mod.log.Info("Found propertyData");
                //    try
                //    {
                //        Mod.log.Info(propertyData.ToJSONString());
                //    }
                //    catch (Exception e)
                //    {
                //        Mod.log.Info(e.Message);
                //    }
                //    if (propertyData.CountProperties(AreaType.Residential) > 0)
                //    {
                //        new TriggerAction(
                //            TriggerType.LevelUpResidentialBuilding,
                //            Entity.Null,
                //            entity,
                //            entity,
                //            0f
                //        );
                //        Mod.log.Info("LevelUpResidentialBuilding");
                //    }
                //    if (propertyData.CountProperties(AreaType.Commercial) > 0)
                //    {
                //        new TriggerAction(
                //            TriggerType.LevelUpCommercialBuilding,
                //            Entity.Null,
                //            entity,
                //            entity,
                //            0f
                //        );
                //        Mod.log.Info("LevelUpCommercialBuilding");
                //    }
                //    if (propertyData.CountProperties(AreaType.Industrial) > 0)
                //    {
                //        if (
                //            entityManager.TryGetComponent(
                //                entity,
                //                out Game.Buildings.OfficeProperty _
                //            )
                //        )
                //        {
                //            new TriggerAction(
                //                TriggerType.LevelUpOfficeBuilding,
                //                Entity.Null,
                //                entity,
                //                entity,
                //                0f
                //            );
                //            Mod.log.Info("LevelUpOfficeBuilding");
                //        }
                //        else
                //        {
                //            new TriggerAction(
                //                TriggerType.LevelUpIndustrialBuilding,
                //                Entity.Null,
                //                entity,
                //                entity,
                //                0f
                //            );
                //            Mod.log.Info("LevelUpIndustrialBuilding");
                //        }
                //    }
                //    new ZoneBuiltLevelUpdate
                //    {
                //        m_Zone = spawnableBuildingData.m_ZonePrefab,
                //        m_FromLevel = (int)spawnableBuildingData.m_Level,
                //        m_ToLevel = (int)(level),
                //        m_Squares = buildingData.m_LotSize.x * buildingData.m_LotSize.y
                //    }
                //}

                string prefix = "StarQ ABM ";
                prefabSystem.TryGetPrefab(prefab, out PrefabBase currentPrefabBase);
                string ogName =
                    $"{Regex.Replace(currentPrefabBase.name.Replace(prefix, ""), @"\s\d{9}$", "")}";
                // ogName in case the mod already replaced the PrefabRef once
                string vName = $"{prefix}{ogName}";

                prefabSystem.TryGetPrefab(
                    new PrefabID("BuildingPrefab", ogName),
                    out PrefabBase ogPrefabBase
                );

                Entity ogPrefabEntity = prefabSystem.GetEntity(ogPrefabBase);

                int ver = 1;
                while (
                    prefabSystem.TryGetPrefab(
                        new PrefabID("BuildingPrefab", $"{vName} {ver:D9}"),
                        out _
                    )
                )
                    ver++;
                PrefabBase newPrefabBase = prefabSystem.DuplicatePrefab(
                    currentPrefabBase,
                    $"{vName} {ver:D9}"
                );
                Entity newEntity = prefabSystem.GetEntity(newPrefabBase);
                if (
                    entityManager.TryGetComponent(
                        ogPrefabEntity,
                        out SpawnableBuildingData spawnableBuildingData
                    )
                    && entityManager.TryGetComponent(
                        ogPrefabEntity,
                        out ConsumptionData consumptionData
                    )
                )
                {
                    spawnableBuildingData.m_ZonePrefab = DataRetriever
                        .zoneDataInfos[new Random().Next(0, DataRetriever.zoneDataInfos.Count)]
                        .Entity;
                    //entityManager.AddComponent<Updated>(newEntity);

                    entityManager.TryGetComponent(
                        ogPrefabEntity,
                        out Game.Prefabs.BuildingData buildingData
                    );

                    var match = DataRetriever.zoneDataInfos.FirstOrDefault(b =>
                        b.PrefabName
                        == prefabSystem.GetPrefabName(spawnableBuildingData.m_ZonePrefab)
                    );

                    entityManager.TryGetComponent(
                        ogPrefabEntity,
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

                    //entityManager.AddComponentData(newEntity, spawnableBuildingData);
                    //entityManager.AddComponentData(newEntity, consumptionData);
                    PrefabRef newPrefabEntity = new() { m_Prefab = newEntity };
                    entityManager.AddComponentData(newPrefabEntity.m_Prefab, spawnableBuildingData);
                    entityManager.AddComponentData(newPrefabEntity.m_Prefab, consumptionData);
                    entityManager.AddComponentData(entity, newPrefabEntity);
                    entityManager.AddComponent<Updated>(newPrefabEntity.m_Prefab);
                    entityManager.AddComponent<Updated>(newEntity);
                    entityManager.AddComponent<Updated>(entity);

                    //TODO: Store the ogPrefab info in a component and reload them on game load, also save to disk.
                }
            }
            catch (Exception ex)
            {
                Mod.log.Info(ex.ToString());
            }
        }

        protected override void OnUpdate() { }
    }
}
