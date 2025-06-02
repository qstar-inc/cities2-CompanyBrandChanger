using System;
using System.Linq;
using Colossal.Entities;
using CompanyBrandChanger.Systems.UI;
using Game.Buildings;
using Game.Common;
using Game.Companies;
using Game.UI;
using Unity.Entities;
using static Game.UI.NameSystem;

namespace CompanyBrandChanger.Systems
{
    public partial class Panel : ExtendedUISystemBase
    {
        private ValueBindingHelper<string> SelectedEntityValue;
        private ValueBindingHelper<string> CurrentBrandValue;
        private ValueBindingHelper<BrandDataInfo[]> BrandDataInfoArray;
        private bool isBrandDataSet;

        protected override void OnCreate()
        {
            base.OnCreate();
            SelectedEntityValue = CreateBinding("SelectedEntity", "-1:-1");
            CurrentBrandValue = CreateBinding("CurrentBrand", "Unknown");
            BrandDataInfoArray = CreateBinding("BrandDataInfoArray", new BrandDataInfo[0]);
            CreateTrigger<string, string>("SetBrand", SetBrand);
            isBrandDataSet = false;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            SelectedEntityValue.Value =
                $"{SIPCompanySectionBrand.SelectedEntity.Index}:{SIPCompanySectionBrand.SelectedEntity.Version}";
            CurrentBrandValue.Value = SIPCompanySectionBrand.CurrentBrandName;
            if (!isBrandDataSet || BrandDataRetriever.hasNewData)
            {
                isBrandDataSet = true;
                BrandDataRetriever.hasNewData = false;
                BrandDataInfoArray.Value = BrandDataRetriever.brandDataInfos.ToArray();
                Mod.log.Info("brandDataInfoArray sent");
            }
        }

        public void SetBrand(string replaceBrand, string entityId)
        {
            try
            {
                var match = BrandDataRetriever.brandDataInfos.FirstOrDefault(b =>
                    b.PrefabName == replaceBrand
                );
                if (
                    entityId == string.Empty
                    || replaceBrand == string.Empty
                    || !entityId.Contains(":")
                    || match == null
                )
                    return;

                string[] parts = entityId.Split(':');
                int index = int.Parse(parts[0]);
                int version = int.Parse(parts[1]);

                Entity propertyEntity = new() { Index = index, Version = version };

                if (EntityManager.Exists(propertyEntity))
                {
                    EntityManager.TryGetBuffer(
                        propertyEntity,
                        false,
                        out DynamicBuffer<Renter> renters
                    );

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
                            CurrentBrandValue.Value = match.Name;
                            Mod.log.Info($"{match.Name} set to Entity {entityId} successfully");

                            EntityManager.SetComponentData(renterEntity, companyData);
                            EntityManager.AddComponent<Updated>(propertyEntity);
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
    }
}
