import { ChangeLevel, InfoButton, levelPanelVisibleBinding } from "bindings";
import { useValue } from "cs2/api";
import { FOCUS_AUTO, FocusDisabled } from "cs2/input";
import { useLocalization } from "cs2/l10n";
import { Button, PanelSection, PanelSectionRow } from "cs2/ui";
import { FC, useMemo } from "react";
import {
  styleLevelProgress,
  styleLevelSection,
  styleProgress,
} from "styleBindings";
import { LocaleKeys, ZoneDataInfo } from "types";

import styles from "./BrandPanel.module.scss";
import { PanelBase } from "./PanelBase";

interface LevelPanelProps {
  h_level: boolean;
  w_level: number;
  w_upkeep: number;
  w_zone: string;
  w_zonelist: ZoneDataInfo[];
}

export const LevelPanel: FC<LevelPanelProps> = (props: LevelPanelProps) => {
  const { translate } = useLocalization();
  const visibleBindingValue = useValue(levelPanelVisibleBinding);

  const headerText = translate(LocaleKeys.ZONING_HEADER);
  const changeLevelText = translate(LocaleKeys.ZONING_CHANGE_LEVEL);
  const descText = translate(LocaleKeys.ZONING_DESCRIPTION);
  const currentUpkeepText = translate(LocaleKeys.ZONING_CURRENT_UPKEEP);

  const ChangeLevelPanel = (level: number) => {
    ChangeLevel(level);
  };

  const visible = useMemo(
    () => visibleBindingValue && props.h_level,
    [visibleBindingValue]
  );
  console.log(props.w_zone);
  return (
    <>
      <PanelBase
        header={headerText!}
        visible={visible}
        content={
          <>
            <PanelSection>
              <PanelSectionRow left={descText} right={""} />
            </PanelSection>
            <PanelSection>
              <PanelSectionRow
                uppercase={true}
                disableFocus={true}
                left={changeLevelText}
                right={
                  <FocusDisabled>
                    <div className={styleLevelSection.bar}>
                      {Array.from({ length: 6 }, (_, i) => {
                        let isPassed = i <= props.w_level;
                        return (
                          <Button
                            key={i}
                            focusKey={FOCUS_AUTO}
                            onSelect={() => ChangeLevelPanel(i)}
                            className={`${styleLevelProgress.progressBar} ${styleLevelSection.levelSegment}`}
                            style={{
                              border: "none",
                              display: "flex",
                              flexDirection: "column",
                            }}
                          >
                            <div
                              className={styleProgress.progressBounds}
                              style={{
                                width: "100%",
                                backdropFilter: `brightness(${i / 6})`,
                              }}
                            >
                              <div
                                className={
                                  isPassed ? styleLevelProgress.progress : ""
                                }
                                style={{
                                  margin: "auto",
                                  textAlign: "center",
                                  width: isPassed ? "100%" : "100%",
                                  color: isPassed ? undefined : "whitesmoke",
                                }}
                              >
                                {i}
                              </div>
                            </div>
                          </Button>
                        );
                      })}
                    </div>
                  </FocusDisabled>
                }
              />
            </PanelSection>
            <PanelSection>
              <PanelSectionRow
                uppercase={true}
                left={currentUpkeepText}
                right={props.w_upkeep}
              />
            </PanelSection>
            <PanelSection>
              <InfoButton
                label={"Manage"}
                selected={true}
                onSelect={() => console.log("AA")}
              />
            </PanelSection>
            <PanelSection>
              {props.w_zonelist
                .sort((a, b) => {
                  const nameA = translate(a.Name) ?? "";
                  const nameB = translate(b.Name) ?? "";
                  return nameA.localeCompare(nameB);
                })
                .map((item, index) => {
                  const isCurrent = item.PrefabName === props.w_zone;
                  const translatedName = translate(item.Name) ?? "";
                  if (translatedName != "" && item.Icon != "") {
                    return (
                      <div
                        onClick={() => {
                          console.log("Clicked " + translatedName);
                        }}
                      >
                        <PanelSectionRow
                          // className={brandRowClass}
                          left={
                            <>
                              <img
                                className={styles.BrandImage}
                                src={`${item.Icon}`}
                              />

                              {isCurrent && (
                                <span className={styles.BrandCurrent}>
                                  [Current]{" "}
                                </span>
                              )}
                              <span className={styles.BrandName}>
                                {translatedName}
                              </span>
                            </>
                          }
                          right={
                            <>
                              {[item.Color1, item.Color2].map((color, i) => (
                                <div
                                  key={i}
                                  className={styles.BrandColorBox}
                                  style={{
                                    background: color.slice(0, -2) + "FF",
                                  }}
                                />
                              ))}
                            </>
                          }
                        />
                      </div>
                    );
                  }
                })}
            </PanelSection>
          </>
        }
      />
    </>
  );
};
