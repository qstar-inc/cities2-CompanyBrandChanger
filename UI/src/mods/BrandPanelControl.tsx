import { bindLocalValue } from "cs2/api";

export const panelVisibleBinding = bindLocalValue(false);

export const panelTrigger = (state: boolean) => {
  panelVisibleBinding.update(state);
};
