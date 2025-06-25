using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdvancedBuildingManager.Components;
using Colossal.Entities;
using Colossal.Serialization.Entities;
using Game;
using Game.Common;
using Game.Prefabs;
using Game.UI.InGame;
using Unity.Collections;
using Unity.Entities;

namespace AdvancedBuildingManager.Systems
{
    public partial class SaveComponents : GameSystemBase
    {
        public string prefix = "StarQ ABM ";
        public SortedDictionary<string, List<(string Name, Entity entity)>> variantsInSave;
        private LevelSystem levelSystem;
        private PrefabSystem prefabSystem;

        protected override void OnCreate()
        {
            Mod.log.Info("OnCreate");
            levelSystem = World.GetOrCreateSystemManaged<LevelSystem>();
            prefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();
            variantsInSave = new();
        }

        //protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        //{
        //    base.OnGameLoadingComplete(purpose, mode);
        protected override void OnUpdate()
        {
            Mod.log.Info("OnGameLoadingComplete");
            variantsInSave.Clear();

            EntityQuery variantsQuery = SystemAPI
                .QueryBuilder()
                .WithAll<BuildingVariants>()
                .Build();
            NativeArray<Entity> variantsEntities = variantsQuery.ToEntityArray(Allocator.Temp);

            foreach (var entity in variantsEntities)
            {
                if (
                    EntityManager.TryGetComponent<BuildingVariants>(
                        entity,
                        out BuildingVariants variant
                    )
                )
                {
                    string ogName = variant.OGName.ToString();
                    string name = variant.Name.ToString();
                    if (!variantsInSave.TryGetValue(ogName, out var variantList))
                    {
                        variantList = new List<(string, Entity)>();
                        variantsInSave[ogName] = variantList;
                    }

                    if (!variantList.Any(entry => entry.Name == name && entry.entity == entity))
                    {
                        variantList.Add((name, entity));
                    }
                }
            }

            foreach (var kvp in variantsInSave)
            {
                string ogName = kvp.Key;
                List<(string, Entity)> variants = kvp.Value;
                prefabSystem.TryGetPrefab(
                    new PrefabID("BuildingPrefab", ogName),
                    out PrefabBase currentPrefabBase
                );

                foreach (var item in variants)
                {
                    int ver = GetVersion(item.Item1);

                    CreateVariants(EntityManager, prefabSystem, item.Item2, currentPrefabBase, ver);
                    Mod.log.Info($"Loaded ver {ver} for {ogName}");
                }
            }
        }

        private int GetVersion(string og)
        {
            int lastSpaceIndex = og.LastIndexOf(' ');
            if (lastSpaceIndex == -1)
                return 0;

            string numberPart = og[(lastSpaceIndex + 1)..];

            if (ulong.TryParse(numberPart, out ulong number))
            {
                return (int)number;
            }

            return 0;
        }

        public void CreateVariants(
            EntityManager entityManager,
            PrefabSystem prefabSystem,
            Entity entity,
            PrefabBase currentPrefabBase,
            int ver
        )
        {
            string ogName =
                $"{Regex.Replace(currentPrefabBase.name.Replace(prefix, ""), @"\s\d{9}$", "")}";
            // ogName in case the mod already replaced the PrefabRef once
            string vName = $"{prefix}{ogName}";

            string newName = $"{vName} {ver:D9}";

            PrefabBase newPrefabBase = prefabSystem.DuplicatePrefab(currentPrefabBase, newName);
            prefabSystem.AddPrefab(newPrefabBase);
            Entity newEntity = prefabSystem.GetEntity(newPrefabBase);

            CopyComponent<BuildingVariants>(entityManager, entity, newEntity);
            CopyComponent<SpawnableBuildingData>(entityManager, entity, newEntity);

            //List<ComponentBase> comps1 = currentPrefabBase.components;
            //List<ComponentBase> comps2 = newPrefabBase.components;

            //foreach (ComponentBase item in comps1)
            //{
            //    prefabSystem.currentPrefabBase.GetComponent<item>(entity, out item oldComp);
            //    entityManager.AddComponent(newEntity, oldComp);
            //}

            entityManager.AddComponent<Updated>(newEntity);
            entityManager.AddComponent<Updated>(entity);
            entityManager.AddComponentData(entity, new PrefabRef() { m_Prefab = newEntity });

            Mod.log.Info($"Created {newName} from {ogName}");

            //TODO: Store the ogPrefab info in a component and reload them on game load, also save to disk.
        }

        void CopyComponent<T>(EntityManager em, Entity from, Entity to)
            where T : unmanaged, IComponentData
        {
            if (em.HasComponent<T>(from))
            {
                var data = em.GetComponentData<T>(from);
                if (!em.HasComponent<T>(to))
                    em.AddComponent<T>(to);
                em.SetComponentData(to, data);
            }
        }
    }
}
