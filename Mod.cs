using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using CompanyBrandChanger.Systems;
using Game;
using Game.Modding;
using Game.SceneFlow;
using Unity.Entities;

namespace CompanyBrandChanger
{
    public class Mod : IMod
    {
        public static string Name = "Company Brand Changer";
        public static string Id = "CompanyBrandChanger";
        public static ILog log = LogManager
            .GetLogger($"{nameof(CompanyBrandChanger)}")
            .SetShowsErrorsInUI(false);
        private Setting m_Setting;

        public void OnLoad(UpdateSystem updateSystem)
        {
            log.Info(nameof(OnLoad));

            if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
                log.Info($"Current mod asset at {asset.path}");

            m_Setting = new Setting(this);
            m_Setting.RegisterInOptionsUI();
            GameManager.instance.localizationManager.AddSource("en-US", new LocaleEN(m_Setting));

            AssetDatabase.global.LoadSettings(
                nameof(CompanyBrandChanger),
                m_Setting,
                new Setting(this)
            );

            //World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<BrandDataRetriever>();
            updateSystem.UpdateAfter<BrandDataRetriever>(SystemUpdatePhase.PrefabUpdate);
            updateSystem.UpdateAfter<SIPCompanySectionBrand>(SystemUpdatePhase.UIUpdate);
            updateSystem.UpdateAfter<Panel>(SystemUpdatePhase.UIUpdate);
        }

        public void OnDispose()
        {
            log.Info(nameof(OnDispose));
            if (m_Setting != null)
            {
                m_Setting.UnregisterInOptionsUI();
                m_Setting = null;
            }
        }
    }
}
