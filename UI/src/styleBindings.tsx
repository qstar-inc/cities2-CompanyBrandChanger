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

export const styleScrollable = getModule(
  "game-ui/common/scrolling/scrollable.module.scss",
  "classes"
);

export const styleSIPThemed = getModule(
  "game-ui/game/themes/selected-info-panel.module.scss",
  "classes"
);

export const styleTheme = getModule(
  "game-ui/common/input/text/ellipsis-text-input/themes/default.module.scss",
  "classes"
);

export const styleLabel = getModule(
  "game-ui/common/input/text/ellipsis-text-input/ellipsis-text-input.module.scss",
  "classes"
);

export const styleCloseButton = getModule(
  "game-ui/common/input/button/themes/round-highlight-button.module.scss",
  "classes"
);
