import { ModRegistrar } from "cs2/modding";
import { AdvancedBuildingManagerSIP } from "mods/SIPSection";

const register: ModRegistrar = (moduleRegistry) => {
  moduleRegistry.extend(
    "game-ui/game/components/selected-info-panel/selected-info-sections/selected-info-sections.tsx",
    "selectedInfoSectionComponents",
    AdvancedBuildingManagerSIP
  );
};

export default register;
