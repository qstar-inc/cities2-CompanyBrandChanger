using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdvancedBuildingManager.Components;
using Colossal.Entities;
using Colossal.IO.AssetDatabase;
using Colossal.PSI.Environment;
using Colossal.Serialization.Entities;
using Game;
using Game.Buildings;
using Game.Common;
using Game.Prefabs;
using Game.Routes;
using Game.UI.InGame;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using static Colossal.AssetPipeline.Diagnostic.Report;

namespace AdvancedBuildingManager.Systems
{
    public partial class SaveComponents : GameSystemBase
    {
        public string prefix = "StarQ ABM ";
        public SortedDictionary<string, List<(string Name, Entity entity)>> variantsInSave;
        private LevelSystem levelSystem;
        private PrefabSystem prefabSystem;
        private static readonly string abmSubpath = "ModsData\\ABM\\.Prefabs";

        protected override void OnCreate()
        {
            Mod.log.Info("OnCreate");
            levelSystem = World.GetOrCreateSystemManaged<LevelSystem>();
            prefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();
            variantsInSave = new();
            string abmPathFull =
                EnvPath.kUserDataPath.TrimEnd('/').TrimEnd('\\') + "/" + abmSubpath;
            if (!Directory.Exists(abmPathFull))
            {
                Directory.CreateDirectory(abmPathFull);
                Mod.log.Info($"Created {abmPathFull}");
            }
            else
            {
                Mod.log.Info($"Found {abmPathFull}");
            }
            Mod.log.Info($"Ready {abmPathFull}");

            foreach (string s in DefaultAssetFactory.instance.GetSupportedExtensions())
            {
                foreach (
                    string file in Directory.GetFiles(
                        abmPathFull,
                        $"*{s}",
                        SearchOption.AllDirectories
                    )
                )
                {
                    string relativeDir = Path.GetRelativePath(
                        abmSubpath,
                        Path.GetDirectoryName(file)
                    );
                    AssetDataPath assetDataPath = AssetDataPath.Create(
                        relativeDir,
                        Path.GetFileName(file),
                        true,
                        EscapeStrategy.None
                    );
                    AssetDatabase.user.AddAsset(assetDataPath);
                    Mod.log.Info($"Loading {file}");
                }
            }
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
            Entity mainEntity,
            PrefabBase currentPrefabBase,
            int ver
        )
        {
            //BuildingPrefab currentPrefab = currentPrefabBase as BuildingPrefab;
            string ogName =
                $"{Regex.Replace(currentPrefabBase.name.Replace(prefix, ""), @"\s\d{9}$", "")}";
            // ogName in case the mod already replaced the PrefabRef once
            string vName = $"{prefix}{ogName}";

            string newName = $"{vName} {ver:D9}";

            if (
                prefabSystem.TryGetPrefab(
                    new PrefabID("BuildingPrefab", newName),
                    out PrefabBase createdPrefabBase
                )
            )
            {
                Entity createdNewRef = prefabSystem.GetEntity(createdPrefabBase);
                entityManager.AddComponentData(
                    mainEntity,
                    new PrefabRef() { m_Prefab = createdNewRef }
                );
                entityManager.AddComponent<Updated>(createdNewRef);
                return;
            }

            BuildingPrefab newPrefabBase = (BuildingPrefab)
                prefabSystem.DuplicatePrefab(currentPrefabBase, newName);

            AssetDataPath adp = AssetDataPath.Create(
                $"{abmSubpath}/{newName}",
                newName ?? "",
                EscapeStrategy.None
            );
            AssetDatabase.user.AddAsset(adp, newPrefabBase).Save(ContentType.Binary, true);

            //entityManager.TryGetComponent(mainEntity, out PrefabRef oldRef);
            Entity newRef = prefabSystem.GetEntity(newPrefabBase);
            //int i = 0;

            //Entity newPBEntity = prefabSystem.GetEntity(newPrefabBase);
            //entityManager.TryGetComponent(newPBEntity, out PrefabData pdO);
            ////entityManager.TryGetComponent(newRef, out PrefabData pdN);
            ////pdN.m_Index = pdO.m_Index;
            //entityManager.SetComponentData(newRef, pdO);

            //entityManager.AddComponent<Created>(newRef);

            //BuildingData buildingData = new()
            //{
            //    m_LotSize = new int2(currentPrefab.m_LotWidth, currentPrefab.m_LotWidth),
            //};
            //entityManager.TryGetComponent(oldRef, out BuildingData c1);
            //entityManager.SetComponentData(newRef, c1);
            //entityManager.TryGetComponent(oldRef, out PlaceableObjectData c2);
            //entityManager.SetComponentData(newRef, c2);
            //entityManager.TryGetComponent(oldRef, out BuildingTerraformData c3);
            //entityManager.SetComponentData(newRef, c3);
            //entityManager.TryGetComponent(oldRef, out ObjectGeometryData c4);
            //entityManager.SetComponentData(newRef, c4);
            //entityManager.TryGetComponent(oldRef, out ObjectData c5);
            //entityManager.SetComponentData(newRef, c5);
            //entityManager.TryGetSharedComponent(oldRef, out BuildingSpawnGroupData c6);
            //entityManager.SetSharedComponentManaged(newRef, c6);

            //PrefabData prefabData = new() { m_Index = newRef.Index };
            //entityManager.SetComponentData(newRef, prefabData);
            //if (entityManager.TryGetBuffer(oldRef, false, out DynamicBuffer<Effect> b1o))
            //{
            //    var b1n = entityManager.AddBuffer<Effect>(newRef);
            //    b1n = b1o;
            //}

            //CopyComponent<BuildingVariants>(entityManager, entity, newEntity);
            //CopyComponent<SpawnableBuildingData>(entityManager, oldRef.m_Prefab, newEntity);
            //CopyComponent<BuildingData>(entityManager, oldRef.m_Prefab, newEntity);
            //CopyComponent<BuildingData>(entityManager, oldRef.m_Prefab, newEntity);

            //entityManager.SetComponentData(
            //    newEntity,
            //    entityManager.GetComponentData<PrefabData>(oldRef.m_Prefab)
            //);

            //List<ComponentBase> comps1 = currentPrefabBase.components;
            //List<ComponentBase> comps2 = newPrefabBase.components;

            //foreach (ComponentBase item in comps1)
            //{
            //    prefabSystem.currentPrefabBase.GetComponent<item>(entity, out item oldComp);
            //    entityManager.AddComponent(newEntity, oldComp);
            //}


            //foreach (var comp in newPrefabBase.components)
            //{
            //    comp.Initialize(entityManager, newRef);
            //    comp.LateInitialize(entityManager, newRef);
            //}
            //entityManager.AddComponent<Updated>(newRef);

            entityManager.AddComponentData(mainEntity, new PrefabRef() { m_Prefab = newRef });
            entityManager.AddComponent<Updated>(newRef);
            Mod.log.Info($"Created {newName} from {ogName}");

            //TODO: Store the ogPrefab info in a component and reload them on game load, also save to disk.
        }
    }
}
