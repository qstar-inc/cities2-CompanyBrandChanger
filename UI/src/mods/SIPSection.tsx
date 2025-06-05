import {
    panelTrigger, panelVisibleBinding, RandomizeStyle, selectedEntity, SplitTextToDiv, ToolButton
} from "bindings";
import { useValue } from "cs2/api";
import { SelectedInfoSectionBase } from "cs2/bindings";
import { FOCUS_AUTO, FocusDisabled } from "cs2/input";
import { useLocalization } from "cs2/l10n";
import { FOCUS_DISABLED, PanelSection, PanelSectionRow } from "cs2/ui";
import { BrandDataInfo, LocaleKeys } from "types";

import { BrandPanel } from "./BrandPanel";
import styles from "./BrandPanel.module.scss";

export interface CompanyBrandSection extends SelectedInfoSectionBase {
  w_brand: string;
  w_brandlist: BrandDataInfo[];
  w_company: string;
}

export const CompanyBrandChangerSystem = (componentList: any): any => {
  componentList["CompanyBrandChanger.Systems.SIPCompanySectionBrand"] = (
    e: CompanyBrandSection
  ) => {
    const { translate } = useLocalization();

    const isPanelOpen = useValue(panelVisibleBinding);
    const selectedEntityVal = useValue(selectedEntity);

    const tooltipText = translate(LocaleKeys.TOOLTIP) ?? "TOOLTIP";
    const modNameText = translate(LocaleKeys.NAME) ?? "NAME";
    const tooltipRandomizeButton =
      translate(LocaleKeys.RANDOMIZE_TOOLTIP) ?? "RANDOMIZE_TOOLTIP";

    return (
      <>
        <PanelSection tooltip={<SplitTextToDiv text={tooltipText} />}>
          <PanelSectionRow
            uppercase={true}
            disableFocus={true}
            left={modNameText}
            right={
              <>
                <FocusDisabled>
                  <ToolButton
                    id="starq-cbc-dice"
                    focusKey={FOCUS_DISABLED}
                    tooltip={tooltipRandomizeButton}
                    selected={false}
                    className={styles.ToolWhite}
                    src="Media/Glyphs/Dice.svg"
                    onSelect={() => {
                      RandomizeStyle(selectedEntityVal);
                    }}
                  />
                  <ToolButton
                    focusKey={FOCUS_AUTO}
                    selected={isPanelOpen}
                    src="Media/Tools/Net Tool/Replace.svg"
                    onSelect={() => {
                      panelTrigger(!isPanelOpen);
                    }}
                  />
                </FocusDisabled>
              </>
            }
          />
        </PanelSection>
        <BrandPanel
          key={selectedEntityVal.index}
          w_brand={e.w_brand}
          w_brandlist={e.w_brandlist}
          w_company={e.w_company}
          w_entity={selectedEntityVal}
        />
      </>
    );
  };
  return componentList as any;
};
