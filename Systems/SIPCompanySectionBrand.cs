using Colossal.Entities;
using Colossal.Serialization.Entities;
using Colossal.UI.Binding;
using Game;
using Game.Buildings;
using Game.Companies;
using Game.Prefabs;
using Game.UI;
using Game.UI.InGame;
using Unity.Entities;

namespace CompanyBrandChanger.Systems
{
    public partial class SIPCompanySectionBrand : InfoSectionBase
    {
        protected override string group
        {
            get { return "SIPCompanySectionBrand"; }
        }

        public static Entity SelectedEntity { get; set; }
        public static string CurrentBrandName { get; set; }

        private NameSystem nameSystem;

        public override void OnWriteProperties(IJsonWriter writer) { }

        protected override void OnProcess() { }

        protected override void Reset() { }

        protected override void OnCreate()
        {
            base.OnCreate();
            m_InfoUISystem.AddMiddleSection(this);
            Mod.log.Info("SIPCompanySectionBrand created");
            nameSystem = World.GetOrCreateSystemManaged<NameSystem>();

            Enabled = false;
        }

        protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);
            if (mode == GameMode.Game)
            {
                Enabled = true;
                return;
            }
            Enabled = false;
            return;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (selectedEntity == Entity.Null)
            {
                return;
            }
            visible = Visible();
        }

        private bool Visible()
        {
            if (
                EntityManager.TryGetBuffer<Renter>(
                    selectedEntity,
                    true,
                    out DynamicBuffer<Renter> renters
                )
            )
            {
                for (int i = 0; i < renters.Length; i++)
                {
                    var renter = renters[i];
                    Entity renterEntity = renter.m_Renter;

                    if (EntityManager.TryGetComponent(renterEntity, out CompanyData companyData))
                    {
                        if (companyData.m_Brand.Equals(Entity.Null))
                            continue;
                        SelectedEntity = selectedEntity;

                        CurrentBrandName = nameSystem.GetRenderedLabelName(companyData.m_Brand);
                        return true;
                    }
                }
            }
            SelectedEntity = Entity.Null;
            return false;
        }
    }
}
