using Colossal.Logging;
using CompanyBrandChanger.Extensions;
using CompanyBrandChanger.Systems;
using Game;
using Game.Modding;
using Game.SceneFlow;
using Unity.Entities;

namespace CompanyBrandChanger
{
    public class Mod : IMod
    {
        public static string Id = nameof(CompanyBrandChanger);
        public static ILog log = LogManager
            .GetLogger($"{nameof(CompanyBrandChanger)}")
            .SetShowsErrorsInUI(false);

        public void OnLoad(UpdateSystem updateSystem)
        {
            foreach (var item in new LocaleHelper($"{Id}.Locale.json").GetAvailableLanguages())
            {
                GameManager.instance.localizationManager.AddSource(item.LocaleId, item);
            }

            World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<BrandDataRetriever>();
            updateSystem.UpdateAfter<SIPCompanySectionBrand>(SystemUpdatePhase.UIUpdate);
        }

        public void OnDispose() { }
    }
}
