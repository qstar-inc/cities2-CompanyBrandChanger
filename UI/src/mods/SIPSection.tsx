import {
    panelTrigger, panelVisibleBinding, RandomizeStyle, SplitTextToDiv, ToolButton
} from "bindings";
import { useValue } from "cs2/api";
import { Entity, SelectedInfoSectionBase } from "cs2/bindings";
import { ActiveFocusDiv, FOCUS_AUTO, FocusDisabled, PassiveFocusDiv } from "cs2/input";
import { useLocalization } from "cs2/l10n";
import { FOCUS_DISABLED, PanelSection, PanelSectionRow } from "cs2/ui";
import { BrandDataInfo, LocaleKeys } from "types";

import { BrandPanel } from "./BrandPanel";
import styles from "./BrandPanel.module.scss";

export interface CompanyBrandSection extends SelectedInfoSectionBase {
  w_brand: string;
  w_brandlist: BrandDataInfo[];
  w_company: string;
  w_entity: Entity;
}

export const CompanyBrandChangerSystem = (componentList: any): any => {
  componentList["CompanyBrandChanger.Systems.SIPCompanySectionBrand"] = (
    e: CompanyBrandSection
  ) => {
    const { translate } = useLocalization();

    const isPanelOpen = useValue(panelVisibleBinding);

    const tooltipText = translate(LocaleKeys.TOOLTIP) ?? "TOOLTIP";
    // const tooltipText2 = translate(LocaleKeys.TOOLTIP2) ?? "TOOLTIP2";
    const modNameText = translate(LocaleKeys.NAME) ?? "NAME";
    const tooltipRandomizeButton =
      translate(LocaleKeys.RANDOMIZE_TOOLTIP) ?? "RANDOMIZE_TOOLTIP";

    return (
      <>
        <PanelSection tooltip={<SplitTextToDiv text={tooltipText} />}>
          <PanelSectionRow
            uppercase={true}
            disableFocus={true}
            // subRow={false}
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
                      RandomizeStyle(e.w_entity);
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
          w_brand={e.w_brand}
          w_brandlist={e.w_brandlist}
          w_company={e.w_company}
          w_entity={e.w_entity}
        />
      </>
    );
  };
  return componentList as any;
};
