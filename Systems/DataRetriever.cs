using System;
using System.Collections.Generic;
using Colossal.Json;
using Colossal.Serialization.Entities;
using Game;
using Game.Prefabs;
using Game.UI;
using Game.UI.InGame;
using Game.Zones;
using Unity.Collections;
using Unity.Entities;

namespace AdvancedBuildingManager.Systems
{
    public class BrandDataInfo
    {
        public string Name { get; set; } = "UnknownName";
        public string PrefabName { get; set; } = "UnknownPrefab";
        public string Color1 { get; set; } = "RGBA(0,0,0)";
        public string Color2 { get; set; } = "RGBA(0,0,0)";
        public string Color3 { get; set; } = "RGBA(0,0,0)";
        public Entity Entity { get; set; } = Entity.Null;
        public string Icon { get; set; } = "";
        public string[] Companies { get; set; } = new string[0];
    }

    public class ZoneDataInfo
    {
        public string Name { get; set; } = "UnknownName";
        public string PrefabName { get; set; } = "UnknownPrefab";
        public string Color1 { get; set; } = "RGBA(0,0,0)";
        public string Color2 { get; set; } = "RGBA(0,0,0)";
        public float Upkeep { get; set; } = 0f;
        public Entity Entity { get; set; } = Entity.Null;
        public string Icon { get; set; } = "";
        public AreaType AreaType { get; set; } = AreaType.None;
        public string AreaTypeString { get; set; } = "";
    }

    public partial class DataRetriever : GameSystemBase
    {
#nullable disable
        public PrefabSystem prefabSystem;
        public PrefabUISystem prefabUISystem;
        public NameSystem nameSystem;

#nullable enable

        public EntityQuery brandQuery;
        public int prevBrandEntityCount;
        public static readonly List<BrandDataInfo> brandDataInfos = new();
        public static bool hasNewBrandData = false;

        public EntityQuery zoneQuery;
        public int prevZoneEntityCount;
        public static readonly List<ZoneDataInfo> zoneDataInfos = new();
        public static bool hasNewZoneData = false;

        protected override void OnCreate()
        {
            base.OnCreate();

            prefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();
            prefabUISystem = World.GetOrCreateSystemManaged<PrefabUISystem>();
            nameSystem = World.GetOrCreateSystemManaged<NameSystem>();
            prevBrandEntityCount = 0;
            prevZoneEntityCount = 0;
        }

        protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        {
            try
            {
                base.OnGameLoadingComplete(purpose, mode);
                if (mode != GameMode.Game)
                    return;
                brandQuery = SystemAPI.QueryBuilder().WithAll<BrandData>().Build();
                NativeArray<Entity> brandEntitiesFromQuery = brandQuery.ToEntityArray(
                    Allocator.Temp
                );

                hasNewBrandData = false;
                if (
                    brandDataInfos.Count == 0
                    || prevBrandEntityCount != brandEntitiesFromQuery.Length
                )
                {
                    brandDataInfos.Clear();
                    prevBrandEntityCount = brandEntitiesFromQuery.Length;
                    foreach (var entity in brandEntitiesFromQuery)
                    {
                        prefabSystem.TryGetPrefab(entity, out BrandPrefab brandPrefab);

                        brandPrefab.TryGetExactly(out UIObject uiObject);
                        string icon = "";
                        if (uiObject != null)
                        {
                            icon = uiObject.m_Icon;
                        }
                        else
                        {
                            icon =
                                $"thumbnail://ThumbnailCamera/BrandPrefab/{brandPrefab.name}?width=32&height=32";
                        }

                        CompanyPrefab[] companyPrefabs = brandPrefab.m_Companies;
                        string[] companies = new string[companyPrefabs.Length];
                        for (int i = 0; i < companyPrefabs.Length; i++)
                        {
                            companies[i] = companyPrefabs[i].name;
                        }

                        brandDataInfos.Add(
                            new BrandDataInfo
                            {
                                Name = nameSystem.GetRenderedLabelName(entity),
                                PrefabName = brandPrefab.name,
                                Color1 = brandPrefab.m_BrandColors[0].ToHexCode(),
                                Color2 = brandPrefab.m_BrandColors[1].ToHexCode(),
                                Color3 = brandPrefab.m_BrandColors[2].ToHexCode(),
                                Entity = entity,
                                Icon = icon,
                                Companies = companies,
                            }
                        );
                    }
                    brandDataInfos.Sort(
                        (a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal)
                    );
                    hasNewBrandData = true;
                }

                zoneQuery = SystemAPI.QueryBuilder().WithAll<ZoneData>().Build();
                NativeArray<Entity> zoneEntitiesFromQuery = zoneQuery.ToEntityArray(Allocator.Temp);

                hasNewZoneData = false;
                if (zoneDataInfos.Count == 0 || prevZoneEntityCount != zoneEntitiesFromQuery.Length)
                {
                    zoneDataInfos.Clear();
                    prevZoneEntityCount = zoneEntitiesFromQuery.Length;
                    foreach (var entity in zoneEntitiesFromQuery)
                    {
                        prefabSystem.TryGetPrefab(entity, out ZonePrefab zonePrefab);

                        zonePrefab.TryGetExactly(out UIObject uiObject);
                        string icon = "";
                        if (uiObject != null)
                        {
                            icon = uiObject.m_Icon;
                        }

                        string areaType = zonePrefab.m_AreaType.ToString();

                        if (zonePrefab.m_Office)
                            areaType = "Office";

                        float upkeep = 0f;
                        zonePrefab.TryGetExactly(out ZoneServiceConsumption zoneServiceConsumption);
                        if (zoneServiceConsumption != null)
                        {
                            upkeep = zoneServiceConsumption.m_Upkeep;
                        }

                        prefabUISystem.GetTitleAndDescription(entity, out var titleId, out var _);

                        zoneDataInfos.Add(
                            new ZoneDataInfo
                            {
                                Name = titleId,
                                PrefabName = zonePrefab.name,
                                Color1 = zonePrefab.m_Color.ToHexCode(),
                                Color2 = zonePrefab.m_Edge.ToHexCode(),
                                Upkeep = upkeep,
                                Entity = entity,
                                Icon = icon,
                                AreaType = zonePrefab.m_AreaType,
                                AreaTypeString = areaType,
                            }
                        );
                    }
                    zoneDataInfos.Sort(
                        (a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal)
                    );
                    hasNewZoneData = true;
                }
            }
            catch (Exception ex)
            {
                Mod.log.Error(ex);
            }
            Mod.log.Info(zoneDataInfos.ToJSONString());
        }

        protected override void OnUpdate() { }
    }
}
