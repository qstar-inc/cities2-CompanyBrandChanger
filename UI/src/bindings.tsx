import engine from "cohtml/cohtml";
import { bindLocalValue, bindValue, trigger } from "cs2/api";
import { Entity } from "cs2/bindings";
import { getModule } from "cs2/modding";
import { ButtonProps, ScrollController } from "cs2/ui";
import mod from "mod.json";
import { ReactElement } from "react";

import styles from "./mods/BrandPanel.module.scss";

export const selectedEntity = bindValue<Entity>(
  "selectedInfo",
  "selectedEntity"
);

export const SetBrand = (replaceBrand: string, entity: Entity) => {
  trigger(mod.id, "SetBrand", replaceBrand, entity);
  engine.trigger("audio.playSound", "select-toggle", 1);
};

export const RandomizeStyle = (entity: Entity) => {
  trigger(mod.id, "RandomizeStyle", entity);
  console.log("click");
};

export const ClosePanel = () => {
  panelVisibleBinding.update(false);
  engine.trigger("audio.playSound", "select-item", 1);
};

export const SplitTextToDiv = ({ text }: { text: string }) => {
  const lines = text.split("\r\n");

  if (lines.length === 1) {
    return <>{text}</>;
  }

  return (
    <>
      {lines.map((line, index) => (
        <div
          className={
            index !== lines.length - 1 ? styles.TooltipMarginBottom : undefined
          }
        >
          {line}
        </div>
      ))}
    </>
  );
};

export const panelVisibleBinding = bindLocalValue(false);
export const manageVisibleBinding = bindLocalValue(false);

export const panelTrigger = (state: boolean) => {
  panelVisibleBinding.update(state);
};

interface ToolButtonProps extends ButtonProps {
  src: string;
  tooltip?: string;
}

export const ToolButton = getModule(
  "game-ui/game/components/tool-options/tool-button/tool-button.tsx",
  "ToolButton"
) as React.FC<ToolButtonProps>;

export type SizeProvider = {
  getRenderedRange: () => {
    offset: number;
    size: number;
    startIndex: number;
    endIndex: number;
  };
  getTotalSize: () => number;
};
export type RenderItemFn = (
  itemIndex: number,
  indexInRange: number
) => ReactElement | null;
type RenderedRangeChangedCallback = (
  startIndex: number,
  endIndex: number
) => void;

interface VirtualListProps {
  className?: string;
  controller?: ScrollController;
  direction?: "vertical" | "horizontal";
  onRenderedRangeChange?: RenderedRangeChangedCallback;
  renderItem: RenderItemFn;
  sizeProvider: SizeProvider;
  smooth?: boolean;
  style?: Partial<CSSStyleDeclaration>;
}

export const VanillaVirtualList = getModule(
  "game-ui/common/scrolling/virtual-list/virtual-list.tsx",
  "VirtualList"
) as React.FC<VirtualListProps>;

export const useUniformSizeProvider: (
  height: number,
  visible: number,
  extents: number
) => SizeProvider = getModule(
  "game-ui/common/scrolling/virtual-list/virtual-list-size-provider.ts",
  "useUniformSizeProvider"
);
