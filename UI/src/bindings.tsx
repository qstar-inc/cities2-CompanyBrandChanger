import engine from "cohtml/cohtml";
import { bindLocalValue, bindValue, trigger } from "cs2/api";
import { Entity, UISound } from "cs2/bindings";
import { getModule } from "cs2/modding";
import { ButtonProps } from "cs2/ui";
import mod from "mod.json";
import { useState } from "react";

import styles from "./mods/BrandPanel.module.scss";

export const activeSelectionBinding = bindValue<boolean>(
  "selectedInfo",
  "activeSelection"
);

export const SetBrand = (replaceBrand: string, entity: Entity) => {
  trigger(mod.id, "SetBrand", replaceBrand, entity);
  engine.trigger("audio.playSound", UISound.selectToggle, 1);
};

export const RandomizeStyle = (entity: Entity) => {
  trigger(mod.id, "RandomizeStyle", entity);
};

export const ClosePanel = () => {
  panelVisibleBinding.update(false);
  engine.trigger("audio.playSound", UISound.selectItem, 1);
};

export const CloseManagePanel = () => {
  manageVisibleBinding.update(false);
  engine.trigger("audio.playSound", UISound.selectItem, 1);
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
export const maxPanelHeight = bindLocalValue(0);

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
