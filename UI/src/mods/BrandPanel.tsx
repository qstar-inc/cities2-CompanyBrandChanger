import {
    activeSelectionBinding, ClosePanel, maxPanelHeight, panelVisibleBinding, SetBrand,
    SplitTextToDiv
} from "bindings";
import { useValue } from "cs2/api";
import { Entity } from "cs2/bindings";
import { useLocalization } from "cs2/l10n";
import { PanelSection, PanelSectionRow, Portal, Scrollable } from "cs2/ui";
import { FC, useEffect, useMemo, useState } from "react";
import {
    closeButtonClass, closeButtonImageClass, styleCloseButton, styleDefault, styleIcon, stylePanel,
    styleSIP, styleTintedIcon, wrapperClass
} from "styleBindings";
import { BrandDataInfo, LocaleKeys } from "types";

import styles from "./BrandPanel.module.scss";

// import { ManagePanel } from "./ManagePanel";

interface BrandPanelProps {
  w_brand: string;
  w_brandlist: BrandDataInfo[];
  w_company: string;
  w_entity: Entity;
}

export const BrandPanel: FC<BrandPanelProps> = (props: BrandPanelProps) => {
  const { translate } = useLocalization();
  const sipActive = useValue(activeSelectionBinding);
  const visibleBindingValue = useValue(panelVisibleBinding);

  const [sipPanel, setSipPanel] = useState(false);
  const [heightFull, setHeightFull] = useState(0);
  const [heightScroll, setHeightScroll] = useState(0);

  const headerText = translate(LocaleKeys.NAME) ?? "NAME";
  const SelectedEntityTitleText =
    translate(LocaleKeys.SELECTED_ENTITY) ?? "SELECTED_ENTITY";
  const CurrentBrandTitleText =
    translate(LocaleKeys.CURRENT_BRAND) ?? "CURRENT_BRAND";
  const CurrentCompanyTitleText =
    translate(LocaleKeys.CURRENT_COMPANY) ?? "CURRENT_COMPANY";
  const SupportedBrandsText =
    translate(LocaleKeys.SUPPORTED_BRANDS)?.toUpperCase() ?? "SUPPORTED_BRANDS";
  const SupportedBrandsTooltip =
    translate(LocaleKeys.SUPPORTED_BRANDS_TOOLTIP) ??
    "SUPPORTED_BRANDS_TOOLTIP";
  const OtherBrandsText =
    translate(LocaleKeys.OTHER_BRANDS)?.toUpperCase() ?? "OTHER_BRANDS";
  const OtherBrandsTooltip =
    translate(LocaleKeys.OTHER_BRANDS_TOOLTIP) ?? "OTHER_BRANDS_TOOLTIP";
  const BrandGroupHoverText =
    translate(LocaleKeys.BRAND_GROUP_HOVER) ?? "BRAND_GROUP_HOVER";

  const [SupportedBrandsArray, OtherBrandsArray] = useMemo(() => {
    const supported: BrandDataInfo[] = [];
    const other: BrandDataInfo[] = [];

    for (const brand of props.w_brandlist ?? []) {
      if (
        Array.isArray(brand.Companies) &&
        brand.Companies.includes(props.w_company)
      ) {
        supported.push(brand);
      } else {
        other.push(brand);
      }
    }

    return [supported, other];
  }, [props.w_brandlist, props.w_company]);

  const wrapperStyle = { maxHeight: `${heightFull}px` };

  const contentScrollStyle = useMemo(
    () => ({
      maxHeight: `calc(${heightScroll}px - 6rem)`,
    }),
    [heightScroll]
  );

  const visible = useMemo(
    () => visibleBindingValue && sipPanel,
    [visibleBindingValue, sipPanel]
  );

  const calculateHeights = () => {
    const wrapperElement = document.querySelector(
      ".info-layout_BVk"
    ) as HTMLElement | null;
    const infoSectionElement = document.getElementsByClassName(
      "info-row_QQ9"
    )[0] as HTMLElement | undefined;

    const newHeightFull = wrapperElement?.offsetHeight ?? 1600;
    const heightSection = infoSectionElement?.offsetHeight ?? 0;
    const newHeightScroll = newHeightFull - heightSection * 3 - 49;

    maxPanelHeight.update(newHeightFull);
    setHeightFull(newHeightFull);
    setHeightScroll(newHeightScroll);
  };

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

  if (props.w_entity.index === -1 || !visible) return null;

  const BrandSection = ({
    BrandsText,
    BrandsTooltip,
    BrandsArrayX,
  }: {
    BrandsText: string;
    BrandsTooltip: string;
    BrandsArrayX: BrandDataInfo[];
  }) => {
    return (
      <>
        <PanelSection>
          <PanelSectionRow
            left={BrandsText}
            right={BrandGroupHoverText}
            tooltip={BrandsTooltip}
          />
          {Object.entries(BrandsArrayX).map(([key, brand]) => {
            const isCurrent = brand.Name === props.w_brand;
            const brandRowClass = `${isCurrent ? styles.BrandCurrentRow : ""} ${
              styles.BrandRow
            }`;
            return (
              <div
                key={key}
                onClick={() => {
                  SetBrand(brand.PrefabName, props.w_entity);
                }}
              >
                <PanelSectionRow
                  className={brandRowClass}
                  left={
                    <>
                      <img
                        className={styles.BrandImage}
                        src={`${brand.Icon}`}
                      />

                      {isCurrent && (
                        <span className={styles.BrandCurrent}>[Current] </span>
                      )}
                      <span className={styles.BrandName}>{brand.Name}</span>
                    </>
                  }
                  right={
                    <>
                      {[brand.Color1, brand.Color2, brand.Color3].map(
                        (color, i) => (
                          <div
                            key={i}
                            className={styles.BrandColorBox}
                            style={{
                              background: color.slice(0, -2) + "FF",
                            }}
                          />
                        )
                      )}
                    </>
                  }
                />
              </div>
            );
          })}
        </PanelSection>
      </>
    );
  };

  return (
    <>
      <Portal>
        <div
          id="starq-cbc-panel"
          className={`${wrapperClass} ${styles.BrandChangerPanel}`}
          style={wrapperStyle}
        >
          <div className={styleDefault.header}>
            <div className={stylePanel.titleBar}>
              <img
                className={stylePanel.icon}
                src="Media/Tools/Net Tool/Replace.svg"
              />
              <div className={styleDefault.title}>{headerText}</div>
              <button className={closeButtonClass} onClick={() => ClosePanel()}>
                <div
                  className={closeButtonImageClass}
                  style={{
                    maskImage: "url(Media/Glyphs/Close.svg)",
                  }}
                ></div>
              </button>
            </div>
          </div>
          <div className={styleDefault.content}>
            <PanelSection>
              <PanelSectionRow
                left={SelectedEntityTitleText}
                right={`${props.w_entity.index}:${props.w_entity.version}`}
              />
              <PanelSectionRow
                left={CurrentBrandTitleText}
                right={props.w_brand}
              />
              <PanelSectionRow
                left={CurrentCompanyTitleText}
                right={props.w_company}
              />
            </PanelSection>
            <PanelSection>
              <Scrollable style={contentScrollStyle}>
                <BrandSection
                  BrandsText={SupportedBrandsText}
                  BrandsTooltip={SupportedBrandsTooltip}
                  BrandsArrayX={SupportedBrandsArray}
                />
                <BrandSection
                  BrandsText={OtherBrandsText}
                  BrandsTooltip={OtherBrandsTooltip}
                  BrandsArrayX={OtherBrandsArray}
                />
              </Scrollable>
            </PanelSection>
          </div>
        </div>
      </Portal>
      {/* <ManagePanel /> */}
    </>
  );
};
