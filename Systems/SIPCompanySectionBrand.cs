using System;
using System.Linq;
using Colossal.Entities;
using Colossal.Serialization.Entities;
using Colossal.UI.Binding;
using CompanyBrandChanger.Extensions;
using Game;
using Game.Buildings;
using Game.Common;
using Game.Companies;
using Game.UI;
using Game.UI.InGame;
using Unity.Entities;
using Unity.Mathematics;

namespace CompanyBrandChanger.Systems
{
    public partial class SIPCompanySectionBrand : ExtendedInfoSectionBase
    {
        public override GameMode gameMode => GameMode.Game;
        protected override string group
        {
            get { return "SIPCompanySectionBrand"; }
        }

# nullable disable
        public static string CurrentBrandName { get; set; }
        public static string CurrentCompanyName { get; set; }
        private NameSystem nameSystem;

#nullable enable

        private Entity companyEntity;
        private bool isBrandDataSet;

        protected override void OnCreate()
        {
            base.OnCreate();
            m_InfoUISystem.AddMiddleSection(this);

            nameSystem = World.GetOrCreateSystemManaged<NameSystem>();
            CreateTrigger<string, Entity>("SetBrand", SetBrand);
            CreateTrigger<Entity>("RandomizeStyle", RandomizeStyle);

            isBrandDataSet = false;
            Enabled = false;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            visible = Visible();
            if (!isBrandDataSet || BrandDataRetriever.hasNewData)
            {
                isBrandDataSet = true;
                BrandDataRetriever.hasNewData = false;
            }
        }

        public override void OnWriteProperties(IJsonWriter writer)
        {
            writer.PropertyName("w_brand");
            writer.Write(CurrentBrandName);

            writer.PropertyName("w_company");
            writer.Write(CurrentCompanyName);

            writer.PropertyName("w_brandlist");
            BrandDataInfoJsonWriterExtensions.Write(
                writer,
                BrandDataRetriever.brandDataInfos.ToArray()
            );
        }

        protected override void OnProcess() { }

        protected override void Reset() { }

        private bool Visible()
        {
            bool isVisible = CompanyUIUtils.HasCompany(
                EntityManager,
                selectedEntity,
                selectedPrefab,
                out companyEntity
            );

            if (!isVisible)
                return false;

            if (EntityManager.TryGetComponent(companyEntity, out CompanyData companyData))
            {
                if (companyData.m_Brand.Equals(Entity.Null))
                    return false;
                CurrentBrandName = nameSystem.GetRenderedLabelName(companyData.m_Brand);
                CurrentCompanyName = nameSystem
                    .GetRenderedLabelName(companyEntity)
                    .Replace("Assets.NAME[", "")
                    .Replace("]", "");
                return true;
            }
            return false;
        }

        public void SetBrand(string replaceBrand, Entity entity)
        {
            try
            {
                var match = BrandDataRetriever.brandDataInfos.FirstOrDefault(b =>
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
                            m_InfoUISystem.SetDirty();
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

        public void RandomizeStyle(Entity entity)
        {
            try
            {
                if (EntityManager.TryGetComponent(entity, out PseudoRandomSeed pseudoRandomSeed))
                {
                    Unity.Mathematics.Random random = new();
                    ushort randomUShort = (ushort)random.NextInt(0, 65536);
                    pseudoRandomSeed.m_Seed = randomUShort;

                    EntityManager.AddComponentData(entity, pseudoRandomSeed);
                    EntityManager.AddComponent<Updated>(entity);
                    m_InfoUISystem.SetDirty();
                }
            }
            catch (Exception ex)
            {
                Mod.log.Info(ex.ToString());
            }
        }
    }
}
