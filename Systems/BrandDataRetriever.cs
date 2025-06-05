using System;
using System.Collections.Generic;
using Colossal.Serialization.Entities;
using Game;
using Game.Prefabs;
using Game.UI;
using Unity.Collections;
using Unity.Entities;

namespace CompanyBrandChanger.Systems
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

    public partial class BrandDataRetriever : GameSystemBase
    {
#nullable disable
        public PrefabSystem prefabSystem;
        public NameSystem nameSystem;
#nullable enable
        public EntityQuery brandQuery;
        public int prevEntityCount;
        public static readonly List<BrandDataInfo> brandDataInfos = new();
        public static bool hasNewData = false;

        protected override void OnCreate()
        {
            base.OnCreate();

            prefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();
            nameSystem = World.GetOrCreateSystemManaged<NameSystem>();
            prevEntityCount = 0;
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

                hasNewData = false;
                if (brandDataInfos.Count == 0 || prevEntityCount != brandEntitiesFromQuery.Length)
                {
                    brandDataInfos.Clear();
                    prevEntityCount = brandEntitiesFromQuery.Length;
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
                    hasNewData = true;
                }
            }
            catch (Exception ex)
            {
                Mod.log.Error(ex);
            }
        }

        protected override void OnUpdate() { }
    }
}
