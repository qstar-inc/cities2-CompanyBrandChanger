import { getModule } from "cs2/modding";
import { ReactNode } from "react";
import mod from "mod.json";
import { ButtonProps } from "cs2/ui";

interface InfoButtonProps {
  label: string;
  icon?: string;
  selected?: boolean;
  onSelect?: () => void;
}
export const InfoButton = getModule(
  "game-ui/game/components/selected-info-panel/shared-components/info-button/info-button.tsx",
  "InfoButton"
) as React.FC<InfoButtonProps>;

interface InfoSectionProps {
  group?: string;
  tooltipKeys?: string[];
  tooltipTags?: string[];
  children?: ReactNode;
}
export const InfoSection = getModule(
  "game-ui/game/components/selected-info-panel/shared-components/info-section/info-section.tsx",
  "InfoSection"
) as React.FC<InfoSectionProps>;

interface ToolButtonProps extends ButtonProps {
  src: string;
  tooltip?: string;
}

export const ToolButton = getModule(
  "game-ui/game/components/tool-options/tool-button/tool-button.tsx",
  "ToolButton"
) as React.FC<ToolButtonProps>;
