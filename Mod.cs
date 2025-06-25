using AdvancedBuildingManager.Components;
using AdvancedBuildingManager.Extensions;
using AdvancedBuildingManager.Systems;
using Colossal.Logging;
using Game;
using Game.Modding;
using Game.SceneFlow;
using Game.Serialization;
using Unity.Entities;

namespace AdvancedBuildingManager
{
    public class Mod : IMod
    {
        public static string Id = nameof(AdvancedBuildingManager);
        public static ILog log = LogManager
            .GetLogger($"{nameof(AdvancedBuildingManager)}")
            .SetShowsErrorsInUI(false);

        public void OnLoad(UpdateSystem updateSystem)
        {
            foreach (var item in new LocaleHelper($"{Id}.Locale.json").GetAvailableLanguages())
            {
                GameManager.instance.localizationManager.AddSource(item.LocaleId, item);
            }

            World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<DataRetriever>();
            updateSystem.UpdateAfter<SaveComponents>(SystemUpdatePhase.Deserialize);
            updateSystem.UpdateAfter<SIPAdvancedBuildingManager>(SystemUpdatePhase.UIUpdate);
        }

        public void OnDispose() { }
    }
}
