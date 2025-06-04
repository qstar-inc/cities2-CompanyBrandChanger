import { getModule } from "cs2/modding";

export const stylePanel = getModule(
  "game-ui/common/panel/panel.module.scss",
  "classes"
);

export const styleDefault = getModule(
  "game-ui/common/panel/themes/default.module.scss",
  "classes"
);

export const styleSIP = getModule(
  "game-ui/game/components/selected-info-panel/selected-info-panel.module.scss",
  "classes"
);

export const styleIcon = getModule(
  "game-ui/common/input/button/icon-button.module.scss",
  "classes"
);

export const styleTintedIcon = getModule(
  "game-ui/common/image/tinted-icon.module.scss",
  "classes"
);

export const styleCloseButton = getModule(
  "game-ui/common/input/button/themes/round-highlight-button.module.scss",
  "classes"
);

export const wrapperClass = `${stylePanel.panel} ${styleSIP.selectedInfoPanel}`;
export const closeButtonClass = `${styleCloseButton.button} ${stylePanel.closeButton}`;
export const closeButtonImageClass = `${styleTintedIcon.tintedIcon} ${styleIcon.icon}`;
