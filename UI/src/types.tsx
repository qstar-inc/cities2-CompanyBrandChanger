import { Entity } from "cs2/bindings";
import mod from "mod.json";

export class LocaleKeys {
  public static PANEL_HEADER: string = `${mod.id}.UI_PANEL[Header]`;
  public static SELECTED_ENTITY: string = `${mod.id}.UI_PANEL[SelectedEntity]`;
  public static CURRENT_BRAND: string = `${mod.id}.UI_PANEL[CurrentBrand]`;
  public static TOOLTIP: string = `${mod.id}.SIP[Tooltip]`;
  public static MOD_NAME: string = `${mod.id}.NAME`;
}

export interface BrandDataInfo {
  Name: string;
  PrefabName: string;
  Color1: string;
  Color2: string;
  Color3: string;
  Entity: Entity;
  Icon: string;
}
