using System;
using System.Linq;
using Colossal.Entities;
using CompanyBrandChanger.Systems.UI;
using Game;
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
        public override GameMode gameMode => GameMode.Game;
        private ValueBindingHelper<Entity> SelectedEntityValue;
        private ValueBindingHelper<string> CurrentBrandValue;
        private ValueBindingHelper<BrandDataInfo[]> BrandDataInfoArray;
        private bool isBrandDataSet;

        protected override void OnCreate()
        {
            base.OnCreate();
            SelectedEntityValue = CreateBinding("SelectedEntity", Entity.Null);
            CurrentBrandValue = CreateBinding("CurrentBrand", "Unknown");
            BrandDataInfoArray = CreateBinding("BrandDataInfoArray", new BrandDataInfo[0]);
            CreateTrigger<string, Entity>("SetBrand", SetBrand);
            isBrandDataSet = false;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            SelectedEntityValue.Value = SIPCompanySectionBrand.SelectedEntity;
            CurrentBrandValue.Value = SIPCompanySectionBrand.CurrentBrandName;
            if (!isBrandDataSet || BrandDataRetriever.hasNewData)
            {
                isBrandDataSet = true;
                BrandDataRetriever.hasNewData = false;
                BrandDataInfoArray.Value = BrandDataRetriever.brandDataInfos.ToArray();
                Mod.log.Info("brandDataInfoArray sent");
            }
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
                            CurrentBrandValue.Value = match.Name;
                            Mod.log.Info($"{match.Name} set successfully");

                            EntityManager.SetComponentData(renterEntity, companyData);
                            EntityManager.AddComponent<Updated>(entity);
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
