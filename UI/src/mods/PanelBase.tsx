import { ClosePanel, selectedEntity } from "bindings";
import { useValue } from "cs2/api";
import { Portal } from "cs2/ui";
import { FC, ReactElement, useEffect, useMemo, useState } from "react";
import {
    closeButtonClass, closeButtonImageClass, styleDefault, stylePanel, wrapperClass
} from "styleBindings";

import styles from "./BrandPanel.module.scss";

interface PanelBase {
  header: string;
  visible: boolean;
  content: ReactElement;
}

export const PanelBase: FC<PanelBase> = (props: PanelBase) => {
  const sE = useValue(selectedEntity);

  const [heightFull, setHeightFull] = useState(0);
  const [panelLeft, setPanelLeft] = useState(0);

  const wrapperStyle = useMemo(
    () => ({
      maxHeight: `${heightFull}px`,
      left: `calc(${panelLeft}px + 20rem)`,
    }),
    [panelLeft, heightFull]
  );

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

  if (sE.index === 0 || !props.visible) return null;

  const animateClass = props.visible ? `${styles.BrandChangerAnimate}` : ``;

  return (
    <>
      <Portal>
        <div
          id="starq-abm-panel"
          className={`${wrapperClass} ${styles.BrandChangerPanel} ${animateClass}`}
          style={wrapperStyle}
        >
          <div className={styleDefault.header}>
            <div className={stylePanel.titleBar}>
              <img
                className={stylePanel.icon}
                src="Media/Tools/Net Tool/Replace.svg"
              />
              <div className={styleDefault.title}>{props.header}</div>
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
            <>{props.content}</>
          </div>
        </div>
      </Portal>
    </>
  );
};
