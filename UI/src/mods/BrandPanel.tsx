import {
    ClosePanel, panelVisibleBinding, selectedEntity, SetBrand, SizeProvider, useUniformSizeProvider,
    VanillaVirtualList
} from "bindings";
import { useValue } from "cs2/api";
import { Entity } from "cs2/bindings";
import { AutoNavigationScope, FocusActivation } from "cs2/input";
import { useLocalization } from "cs2/l10n";
import { PanelSection, PanelSectionRow, Portal } from "cs2/ui";
import { useCssLength } from "cs2/utils";
import { FC, useCallback, useEffect, useMemo, useState } from "react";
import {
    closeButtonClass, closeButtonImageClass, styleDefault, stylePanel, wrapperClass
} from "styleBindings";
import { BrandDataInfo, LocaleKeys } from "types";

import styles from "./BrandPanel.module.scss";

interface BrandPanelProps {
  w_brand: string;
  w_brandlist: BrandDataInfo[];
  w_company: string;
  w_entity: Entity;
}

const BrandSection = ({
  BrandsText,
  BrandsTooltip,
  BrandsArrayX,
  BrandGroupHoverText,
  SelectedBrand,
  Entity,
  MaxHeight,
  SizeProvider,
}: {
  BrandsText: string;
  BrandsTooltip: string;
  BrandsArrayX: BrandDataInfo[];
  BrandGroupHoverText: string;
  SelectedBrand: string;
  Entity: Entity;
  MaxHeight: number;
  SizeProvider: SizeProvider;
}) => {
  const RenderItem = useCallback(
    (itemIndex: number, indexInRange: number) => {
      if (itemIndex < 0 || itemIndex >= BrandsArrayX.length) return null;
      const brand = BrandsArrayX[itemIndex];
      const isCurrent = brand.Name === SelectedBrand;
      const brandRowClass = `${isCurrent ? styles.BrandCurrentRow : ""} ${
        styles.BrandRow
      }`;
      return (
        <RenderRow
          brand={brand}
          entity={Entity}
          isCurrent={isCurrent}
          brandRowClass={brandRowClass}
        />
      );
    },
    [SelectedBrand, Entity, BrandsArrayX]
  );

  return (
    <>
      <PanelSection>
        <PanelSectionRow
          left={BrandsText}
          right={BrandGroupHoverText}
          tooltip={BrandsTooltip}
        />
        <AutoNavigationScope activation={FocusActivation.AnyChildren}>
          <VanillaVirtualList
            direction="vertical"
            sizeProvider={SizeProvider}
            renderItem={RenderItem}
            style={{
              maxHeight: `${Math.min(30 * BrandsArrayX.length, MaxHeight)}rem`,
            }}
            smooth
          />
        </AutoNavigationScope>
      </PanelSection>
    </>
  );
};

export const RenderRow = ({
  entity,
  isCurrent,
  brand,
  brandRowClass,
}: {
  entity: Entity;
  brand: BrandDataInfo;
  isCurrent: boolean;
  brandRowClass: string;
}) => {
  return (
    <div
      onClick={() => {
        SetBrand(brand.PrefabName, entity);
      }}
    >
      <PanelSectionRow
        className={brandRowClass}
        left={
          <>
            <img className={styles.BrandImage} src={`${brand.Icon}`} />

            {isCurrent && (
              <span className={styles.BrandCurrent}>[Current] </span>
            )}
            <span className={styles.BrandName}>{brand.Name}</span>
          </>
        }
        right={
          <>
            {[brand.Color1, brand.Color2, brand.Color3].map((color, i) => (
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
};

export const BrandPanel: FC<BrandPanelProps> = (props: BrandPanelProps) => {
  const { translate } = useLocalization();
  const visibleBindingValue = useValue(panelVisibleBinding);
  const sE = useValue(selectedEntity);

  const [heightFull, setHeightFull] = useState(0);
  const [panelLeft, setPanelLeft] = useState(0);

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

  const wrapperStyle = useMemo(
    () => ({
      maxHeight: `${heightFull}px`,
      left: `calc(${panelLeft}px + 20rem)`,
    }),
    [panelLeft, heightFull]
  );

  const visible = useMemo(() => visibleBindingValue, [visibleBindingValue]);

  const calculateHeights = () => {
    const wrapperElement = document.querySelector(
      ".info-layout_BVk"
    ) as HTMLElement | null;
    const sipElement = document.querySelector(
      ".selected-info-panel_gG8"
    ) as HTMLElement | null;

    const newHeightFull = wrapperElement?.offsetHeight ?? 1600;
    if (sipElement?.offsetWidth == 0) {
      return;
    } else {
      const newPanelLeft =
        (sipElement?.offsetLeft ?? 6) + (sipElement?.offsetWidth ?? 300);
      setPanelLeft(newPanelLeft);
    }
    setHeightFull(newHeightFull);
  };

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

  const sizeProviderSupported = useUniformSizeProvider(
    useCssLength("30rem"),
    SupportedBrandsArray.length,
    5
  );
  const sizeProviderOther = useUniformSizeProvider(
    useCssLength("30rem"),
    OtherBrandsArray.length,
    5
  );

  if (sE.index === 0 || !visible) return null;

  const animateClass = visible ? `${styles.BrandChangerAnimate}` : ``;

  return (
    <>
      <Portal>
        <div
          id="starq-cbc-panel"
          className={`${wrapperClass} ${styles.BrandChangerPanel} ${animateClass}`}
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
              <BrandSection
                BrandsText={SupportedBrandsText}
                BrandsTooltip={SupportedBrandsTooltip}
                BrandsArrayX={SupportedBrandsArray}
                BrandGroupHoverText={BrandGroupHoverText}
                SelectedBrand={props.w_brand}
                Entity={props.w_entity}
                MaxHeight={210}
                SizeProvider={sizeProviderSupported}
              />
              <BrandSection
                BrandsText={OtherBrandsText}
                BrandsTooltip={OtherBrandsTooltip}
                BrandsArrayX={OtherBrandsArray}
                BrandGroupHoverText={BrandGroupHoverText}
                SelectedBrand={props.w_brand}
                Entity={props.w_entity}
                MaxHeight={650 - Math.min(SupportedBrandsArray.length, 7) * 30}
                SizeProvider={sizeProviderOther}
              />
            </PanelSection>
          </div>
        </div>
      </Portal>
    </>
  );
};
