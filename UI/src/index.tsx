import { ModRegistrar } from "cs2/modding";
import { CompanyBrandChangerSystem } from "mods/SIPSection";

const register: ModRegistrar = (moduleRegistry) => {
  moduleRegistry.extend(
    "game-ui/game/components/selected-info-panel/selected-info-sections/selected-info-sections.tsx",
    "selectedInfoSectionComponents",
    CompanyBrandChangerSystem
  );
};

export default register;
