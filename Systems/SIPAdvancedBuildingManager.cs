using System;
using System.Linq;
using AdvancedBuildingManager.Extensions;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game;
using Game.Buildings;
using Game.Common;
using Game.Companies;
using Game.Prefabs;
using Game.UI;
using Game.UI.InGame;
using Unity.Entities;

namespace AdvancedBuildingManager.Systems
{
    public partial class SIPAdvancedBuildingManager : ExtendedInfoSectionBase
    {
        public override GameMode gameMode => GameMode.Game;
        protected override string group
        {
            get { return "SIPAdvancedBuildingManager"; }
        }

# nullable disable
        public static string CurrentBrandName { get; set; }
        public static string CurrentCompanyName { get; set; }
        public static int CurrentLevel { get; set; }
        public static int CurrentUpkeep { get; set; }
        public static string CurrentZoneName { get; set; }
        private NameSystem nameSystem;
        private PrefabSystem prefabSystem;
        private LevelSystem levelSystem;

#nullable enable

        private Entity companyEntity;
        private bool isBrandDataSet;
        private bool hasBrand;
        private bool hasLevel;

        protected override void OnCreate()
        {
            base.OnCreate();
            m_InfoUISystem.AddMiddleSection(this);

            nameSystem = World.GetOrCreateSystemManaged<NameSystem>();
            prefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();
            levelSystem = World.GetOrCreateSystemManaged<LevelSystem>();
            CreateTrigger<string>("SetBrand", SetBrand);
            CreateTrigger("RandomizeStyle", RandomizeStyle);
            CreateTrigger<int>("ChangeLevel", ChangeLevel);

            isBrandDataSet = false;
            Enabled = false;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            visible = Visible();
            if (!isBrandDataSet || DataRetriever.hasNewBrandData)
            {
                isBrandDataSet = true;
                DataRetriever.hasNewBrandData = false;
            }
        }

        public override void OnWriteProperties(IJsonWriter writer)
        {
            writer.PropertyName("h_level");
            writer.Write(hasLevel);

            writer.PropertyName("w_level");
            writer.Write(CurrentLevel);

            writer.PropertyName("w_upkeep");
            writer.Write(CurrentUpkeep);

            writer.PropertyName("w_zone");
            writer.Write(CurrentZoneName);

            writer.PropertyName("w_zonelist");
            ZoneDataInfoJsonWriterExtensions.Write(writer, DataRetriever.zoneDataInfos.ToArray());

            writer.PropertyName("h_brand");
            writer.Write(hasBrand);

            writer.PropertyName("w_brand");
            writer.Write(CurrentBrandName);

            writer.PropertyName("w_company");
            writer.Write(CurrentCompanyName);

            writer.PropertyName("w_brandlist");
            BrandDataInfoJsonWriterExtensions.Write(writer, DataRetriever.brandDataInfos.ToArray());
        }

        protected override void OnProcess() { }

        protected override void Reset() { }

        private bool Visible()
        {
            bool isVisible = false;
            //if (EntityManager.TryGetComponent(selectedEntity, out PrefabRef prefabRef))
            //{
            if (EntityManager.TryGetComponent(selectedPrefab, out BuildingData _))
                isVisible = true;

            hasLevel = false;
            if (
                !EntityManager.HasComponent<SignatureBuildingData>(selectedPrefab)
                && !EntityManager.HasComponent<Abandoned>(selectedEntity)
                && EntityManager.HasComponent<Renter>(selectedEntity)
                && EntityManager.HasComponent<BuildingData>(selectedPrefab)
                && EntityManager.HasComponent<SpawnableBuildingData>(selectedPrefab)
                && EntityManager.TryGetComponent(
                    selectedPrefab,
                    out SpawnableBuildingData spawnableBuildingData
                )
                && EntityManager.TryGetComponent(
                    selectedPrefab,
                    out ConsumptionData consumptionData
                )
            )
            {
                hasLevel = true;
                CurrentLevel = spawnableBuildingData.m_Level;
                CurrentUpkeep = consumptionData.m_Upkeep;
                CurrentZoneName = prefabSystem.GetPrefabName(spawnableBuildingData.m_ZonePrefab);
            }
            //}

            if (!isVisible)
                return false;

            hasBrand = false;
            if (
                CompanyUIUtils.HasCompany(
                    EntityManager,
                    selectedEntity,
                    selectedPrefab,
                    out companyEntity
                )
            )
            {
                if (EntityManager.TryGetComponent(companyEntity, out CompanyData companyData))
                {
                    if (!companyData.m_Brand.Equals(Entity.Null))
                    {
                        CurrentBrandName = nameSystem.GetRenderedLabelName(companyData.m_Brand);
                        CurrentCompanyName = nameSystem
                            .GetRenderedLabelName(companyEntity)
                            .Replace("Assets.NAME[", "")
                            .Replace("]", "");
                        hasBrand = true;
                    }
                }
            }

            return true;
        }

        public void SetBrand(string replaceBrand)
        {
            try
            {
                Entity entity = selectedEntity;
                var match = DataRetriever.brandDataInfos.FirstOrDefault(b =>
                    b.PrefabName == replaceBrand
                );
                if (entity == Entity.Null || replaceBrand == string.Empty || match == null)
                    return;

                if (EntityManager.Exists(entity))
                {
                    EntityManager.TryGetBuffer(entity, false, out DynamicBuffer<Renter> renters);

                    for (int i = 0; i < renters.Length; i++)
                    {
                        var renter = renters[i];
                        Entity renterEntity = renter.m_Renter;

                        if (
                            EntityManager.TryGetComponent(renterEntity, out CompanyData companyData)
                        )
                        {
                            if (companyData.m_Brand.Equals(Entity.Null))
                                continue;
                            companyData.m_Brand = match.Entity;

                            EntityManager.SetComponentData(renterEntity, companyData);
                            EntityManager.AddComponent<Updated>(entity);
                            SetDirty();
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Mod.log.Info(ex.ToString());
            }
        }

        public void RandomizeStyle()
        {
            RandomizeStyleSystem.RandomizeStyle(EntityManager, selectedEntity);
            SetDirty();
        }

        public void ChangeLevel(int level)
        {
            levelSystem.SetLevel(
                EntityManager,
                prefabSystem,
                selectedEntity,
                selectedPrefab,
                level
            );
            SetDirty();
        }

        public void SetDirty()
        {
            m_Dirty = true;
        }
    }
}
