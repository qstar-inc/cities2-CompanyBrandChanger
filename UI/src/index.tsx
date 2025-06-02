import { getModule, ModRegistrar } from "cs2/modding";
import { HelloWorldComponent } from "mods/hello-world";
import { CompanyBrandChangerSystem } from "mods/SIPSection";
import { SelectedInfoSections } from "cs2/bindings";
import { BrandPanel } from "mods/BrandPanel";

const register: ModRegistrar = (moduleRegistry) => {
  console.log("HI_FromStart");
  moduleRegistry.extend(
    "game-ui/game/components/selected-info-panel/selected-info-sections/selected-info-sections.tsx",
    "selectedInfoSectionComponents",
    CompanyBrandChangerSystem
  );

  moduleRegistry.append("Game", BrandPanel);
};

export default register;
