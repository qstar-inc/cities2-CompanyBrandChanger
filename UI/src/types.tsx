import { Entity } from "cs2/bindings";
import mod from "mod.json";

export class LocaleKeys {
  public static CURRENT_BRAND: string = `${mod.id}.UI_PANEL[CurrentBrand]`;
  public static CURRENT_COMPANY: string = `${mod.id}.UI_PANEL[CurrentCompany]`;
  public static NAME: string = `${mod.id}.NAME`;
  public static OTHER_BRANDS: string = `${mod.id}.UI_PANEL[OtherBrands]`;
  public static OTHER_BRANDS_TOOLTIP: string = `${mod.id}.UI_PANEL[OtherBrandsTooltip]`;
  public static SELECTED_ENTITY: string = `${mod.id}.UI_PANEL[SelectedEntity]`;
  public static SUPPORTED_BRANDS: string = `${mod.id}.UI_PANEL[SupportedBrands]`;
  public static SUPPORTED_BRANDS_TOOLTIP: string = `${mod.id}.UI_PANEL[SupportedBrandsTooltip]`;
  public static TOOLTIP: string = `${mod.id}.SIP[Tooltip]`;
  public static BRAND_GROUP_HOVER: string = `${mod.id}.UI_PANEL[BrandGroupHover]`;
  public static RANDOMIZE_TOOLTIP: string = `${mod.id}.SIP[RandomizeTooltip]`;
  public static MANAGE_HEADER: string = `${mod.id}.MANAGE_PANEL[Header]`;
}

export interface BrandDataInfo {
  Name: string;
  PrefabName: string;
  Color1: string;
  Color2: string;
  Color3: string;
  Entity: Entity;
  Icon: string;
  Companies: string[];
}
