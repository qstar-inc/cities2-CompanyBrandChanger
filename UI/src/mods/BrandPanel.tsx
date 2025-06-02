import {
  stylePanel,
  styleDefault,
  styleSIP,
  styleCloseButton,
} from "styleBindings";
import { panelVisibleBinding } from "./BrandPanelControl";
import { useEffect, useState } from "react";
import { bindValue, trigger, useValue } from "cs2/api";
import mod from "mod.json";
import { PanelSection, PanelSectionRow, Scrollable } from "cs2/ui";
import { useLocalization } from "cs2/l10n";
import { BrandDataInfo, LocaleKeys } from "types";
import styles from "./BrandPanel.module.scss";

const activeSelectionBinding = bindValue<boolean>(
  "selectedInfo",
  "activeSelection"
);

export const BrandPanel = () => {
  const { translate } = useLocalization();
  const sipActive = useValue(activeSelectionBinding);

  const CurrentBrandBinding = bindValue<string>(
    mod.id,
    "CurrentBrand",
    "Unknown"
  );

  const SelectedEntityBinding = bindValue<string>(
    mod.id,
    "SelectedEntity",
    "Unknown"
  );
  const CurrentBrandText = useValue(CurrentBrandBinding);
  const SelectedEntityText = useValue(SelectedEntityBinding);

  const [sipPanel, setSipPanel] = useState(false);
  const [heightFull, setHeightFull] = useState(0);
  const [heightScroll, setHeightScroll] = useState(0);

  const SetBrand = (replaceBrand: string, entityId: string) =>
    trigger(mod.id, "SetBrand", replaceBrand, entityId);

  useEffect(() => {
    const el = document.querySelector(
      ".row_OqM.container_Ty2.selected-info-panel_iIe"
    );
    const observer = new MutationObserver(() => {
      setSipPanel(!!el);
    });

    observer.observe(document.body, {
      childList: true,
      subtree: true,
    });
    setSipPanel(!!el);

    return () => observer.disconnect();
  }, []);

  useEffect(() => {
    if (!sipActive && panelVisibleBinding.value) {
      panelVisibleBinding.update(false);
    }
  }, [sipActive]);

  useEffect(() => {
    const calculateHeights = () => {
      const wrapperElement = document.querySelector(
        ".info-layout_BVk"
      ) as HTMLElement | null;
      const infoSectionElement = document.getElementsByClassName(
        "info-row_QQ9"
      )[0] as HTMLElement | undefined;

      const newHeightFull = wrapperElement?.offsetHeight ?? 1600;
      setHeightFull(newHeightFull);
      const heightSection = infoSectionElement?.offsetHeight ?? 0;

      const newHeightScroll = newHeightFull - heightSection * 2 - 49;
      setHeightScroll(newHeightScroll);
    };

    calculateHeights();

    const observer = new MutationObserver(() => {
      calculateHeights();
    });

    observer.observe(document.body, {
      childList: true,
      subtree: true,
    });

    return () => observer.disconnect();
  }, []);

  const visibleBindingValue = useValue(panelVisibleBinding);

  const visible = visibleBindingValue && sipPanel;
  if (!visible) return null;

  const headerText = translate(LocaleKeys.PANEL_HEADER, "PANEL_HEADER");
  const SelectedEntityTitleText = translate(
    LocaleKeys.SELECTED_ENTITY,
    "SELECTED_ENTITY"
  );

  const CurrentBrandTitleText = translate(
    LocaleKeys.CURRENT_BRAND,
    "CURRENT_BRAND"
  );
  const AllBrandsBinding = bindValue<BrandDataInfo[]>(
    mod.id,
    "BrandDataInfoArray"
  );

  const BrandsArray = AllBrandsBinding.value ?? [];

  if (SelectedEntityText == "-1:-1") return null;

  return (
    <div
      className={`${stylePanel.panel} ${styleSIP.selectedInfoPanel} ${styles.BrandChangerPanel}`}
      id="starq-brandchanger-panel"
      style={{
        maxHeight: `${heightFull}px`,
      }}
    >
      <div className={styleDefault.header}>
        <div className={stylePanel.titleBar}>
          <img
            className={stylePanel.icon}
            src="Media/Tools/Net Tool/Replace.svg"
          />
          <div className={styleDefault.title}>{headerText}</div>
          <button
            className={`${styleCloseButton.button} ${stylePanel.closeButton}`}
            onClick={() => {
              panelVisibleBinding.update(false);
              trigger("audio.playSound", "close-menu", 1);
            }}
          >
            <div
              className={`tinted-icon_iKo icon_PhD`}
              style={{
                maskImage: "url(Media/Glyphs/Close.svg)",
              }}
            ></div>
          </button>
        </div>
      </div>
      <div className={styleDefault.content}>
        <div>
          <PanelSection>
            <PanelSectionRow
              left={SelectedEntityTitleText}
              right={SelectedEntityText}
            ></PanelSectionRow>
            <PanelSectionRow
              left={CurrentBrandTitleText}
              right={CurrentBrandText}
            ></PanelSectionRow>
          </PanelSection>
          <PanelSection>
            <Scrollable style={{ maxHeight: `calc(${heightScroll}px - 6rem)` }}>
              {Object.entries(BrandsArray).map(([key, brand]) => (
                <div
                  key={key}
                  onClick={() => {
                    SetBrand(brand.PrefabName, SelectedEntityText);
                    console.log(brand.Icon);
                    console.log(brand.Color1);
                    console.log(brand.Color2);
                    console.log(brand.Color3);
                  }}
                >
                  <PanelSectionRow
                    className={`${
                      brand.Name === CurrentBrandText && styles.BrandCurrentRow
                    } ${styles.BrandRow}`}
                    left={
                      <>
                        {
                          <img
                            className={styles.BrandImage}
                            src={brand.Icon}
                          ></img>
                        }{" "}
                        {brand.Name === CurrentBrandText && (
                          <span className={styles.BrandCurrent}>
                            [Current]{" "}
                          </span>
                        )}
                        <span className={styles.BrandName}>{brand.Name}</span>
                      </>
                    }
                    right={
                      <>
                        <div
                          className={styles.BrandColorBox}
                          style={{
                            background: brand.Color1.slice(0, -2) + "FF",
                          }}
                        ></div>
                        <div
                          className={styles.BrandColorBox}
                          style={{
                            background: brand.Color2.slice(0, -2) + "FF",
                          }}
                        ></div>
                        <div
                          className={styles.BrandColorBox}
                          style={{
                            background: brand.Color3.slice(0, -2) + "FF",
                          }}
                        ></div>
                      </>
                    }
                  ></PanelSectionRow>
                </div>
              ))}
            </Scrollable>
          </PanelSection>
        </div>
      </div>
    </div>
  );
};
