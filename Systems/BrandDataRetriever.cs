using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Colossal.Entities;
using Colossal.IO.AssetDatabase;
using Colossal.Serialization.Entities;
using Game;
using Game.Prefabs;
using Game.UI;
using Game.UI.InGame;
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
    }

    public partial class BrandDataRetriever : GameSystemBase
    {
        public PrefabSystem prefabSystem;
        public NameSystem nameSystem;
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
            base.OnGameLoadingComplete(purpose, mode);
            brandQuery = SystemAPI.QueryBuilder().WithAll<BrandData>().Build();
            NativeArray<Entity> brandEntitiesFromQuery = brandQuery.ToEntityArray(Allocator.Temp);

            if (brandDataInfos.Count == 0 || prevEntityCount != brandEntitiesFromQuery.Length)
            {
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
                        }
                    );
                }
                brandDataInfos.Sort(
                    (a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal)
                );
                hasNewData = true;
                Mod.log.Info($"{brandDataInfos.Count} BrandDataInfos added successfully");
            }
            else
            {
                hasNewData = false;
                Mod.log.Info($"{brandDataInfos.Count} BrandDataInfos loaded from memory");
            }
        }

        protected override void OnUpdate() { }
    }
}
