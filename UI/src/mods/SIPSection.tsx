import {
    brandPanelVisibleBinding, levelPanelVisibleBinding, PanelIndex, RandomizeStyle, selectedEntity,
    togglePanel, ToolButton
} from "bindings";
import { useValue } from "cs2/api";
import { SelectedInfoSectionBase } from "cs2/bindings";
import { FOCUS_AUTO, FocusDisabled } from "cs2/input";
import { useLocalization } from "cs2/l10n";
import { FOCUS_DISABLED, PanelSection, PanelSectionRow } from "cs2/ui";
import { BrandDataInfo, LocaleKeys, ZoneDataInfo } from "types";

import { BrandPanel } from "./BrandPanel";
import styles from "./BrandPanel.module.scss";
import { LevelPanel } from "./LevelPanel";

interface SIPAdvancedBuildingManager extends SelectedInfoSectionBase {
  h_level: boolean;
  w_level: number;
  w_upkeep: number;
  w_zone: string;
  w_zonelist: ZoneDataInfo[];
  w_variant: string;

  h_brand: boolean;
  w_brand: string;
  w_brandlist: BrandDataInfo[];
  w_company: string;
}

export const AdvancedBuildingManagerSIP = (componentList: any): any => {
  componentList["AdvancedBuildingManager.Systems.SIPAdvancedBuildingManager"] =
    (e: SIPAdvancedBuildingManager) => {
      const { translate } = useLocalization();

      const isBrandPanelOpen = useValue(brandPanelVisibleBinding);
      const isLevelPanelOpen = useValue(levelPanelVisibleBinding);
      const selectedEntityVal = useValue(selectedEntity);

      const modNameText = translate(LocaleKeys.NAME) ?? "NAME";
      const tooltipBrandChanger =
        translate(LocaleKeys.BRAND_CHANGER_TOOLTIP) ?? "BRAND_CHANGER_TOOLTIP";
      const tooltipRandomizeButton =
        translate(LocaleKeys.RANDOMIZE_TOOLTIP) ?? "RANDOMIZE_TOOLTIP";

      return (
        <>
          <PanelSection>
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
                        RandomizeStyle();
                      }}
                    />
                    {e.h_brand && (
                      <ToolButton
                        focusKey={FOCUS_AUTO}
                        tooltip={tooltipBrandChanger}
                        selected={isBrandPanelOpen}
                        src="Media/Tools/Net Tool/Replace.svg"
                        onSelect={() => {
                          togglePanel(PanelIndex.Brand);
                        }}
                      />
                    )}
                    {e.h_level && (
                      <ToolButton
                        focusKey={FOCUS_AUTO}
                        selected={isLevelPanelOpen}
                        src="Media/Tools/Net Tool/Straight.svg"
                        onSelect={() => {
                          togglePanel(PanelIndex.Level);
                        }}
                      />
                    )}
                  </FocusDisabled>
                </>
              }
            />
          </PanelSection>
          <BrandPanel
            key={selectedEntityVal.index}
            h_brand={e.h_brand}
            w_brand={e.w_brand}
            w_brandlist={e.w_brandlist}
            w_company={e.w_company}
            w_entity={selectedEntityVal}
          />
          <LevelPanel
            key={selectedEntityVal.index}
            h_level={e.h_level}
            w_level={e.w_level}
            w_upkeep={e.w_upkeep}
            w_zone={e.w_zone}
            w_zonelist={e.w_zonelist}
            w_variant={e.w_variant}
          />
        </>
      );
    };
  return componentList as any;
};
