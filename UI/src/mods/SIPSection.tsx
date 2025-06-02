import { ToolButton } from "bindings";
import { useValue } from "cs2/api";
import { SelectedInfoSectionBase } from "cs2/bindings";
import { panelTrigger, panelVisibleBinding } from "./BrandPanelControl";
import { PanelSection, PanelSectionRow } from "cs2/ui";
import { useLocalization } from "cs2/l10n";
import { LocaleKeys } from "types";

export const CompanyBrandChangerSystem = (componentList: any): any => {
  componentList["CompanyBrandChanger.Systems.SIPCompanySectionBrand"] = (
    e: SelectedInfoSectionBase
  ) => {
    const { translate } = useLocalization();
    const tooltipText = translate(LocaleKeys.TOOLTIP, "TOOLTIP");
    const modNameText = translate(LocaleKeys.MOD_NAME, "MOD_NAME");
    const isPanelOpen = useValue(panelVisibleBinding);
    return (
      <PanelSection tooltip={tooltipText}>
        <PanelSectionRow
          disableFocus={true}
          left={modNameText}
          right={
            <ToolButton
              selected={isPanelOpen}
              src="Media/Tools/Net Tool/Replace.svg"
              onSelect={() => {
                panelTrigger(!isPanelOpen);
              }}
            ></ToolButton>
          }
        />
      </PanelSection>
    );
  };
  return componentList as any;
};
