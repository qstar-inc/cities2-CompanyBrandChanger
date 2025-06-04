// import { CloseManagePanel, maxPanelHeight } from "bindings";
// import { useValue } from "cs2/api";
// import { useLocalization } from "cs2/l10n";
// import { PanelSection, PanelSectionRow, Portal } from "cs2/ui";
// import { useState } from "react";
// import {
//     closeButtonClass, closeButtonImageClass, styleDefault, stylePanel, wrapperClass
// } from "styleBindings";
// import { LocaleKeys } from "types";

// import styles from "./BrandPanel.module.scss";

// export const ManagePanel = () => {
//   const { translate } = useLocalization();

//   const maxPanelHeightValue = useValue(maxPanelHeight);

//   const headerText = translate(LocaleKeys.MANAGE_HEADER) ?? "MANAGE_HEADER";

//   const wrapperStyle = { maxHeight: `${maxPanelHeightValue}px` };

//   return (
//     <Portal>
//       <div
//         id="starq-cbc-manage"
//         className={`${wrapperClass} ${styles.BrandChangerManage}`}
//         style={wrapperStyle}
//       >
//         <div className={styleDefault.header}>
//           <div className={stylePanel.titleBar}>
//             <img
//               className={stylePanel.icon}
//               src="Media/Game/Misc/DemandFactorPositive.svg"
//             />
//             <div className={styleDefault.title}>{headerText}</div>
//             <button
//               className={closeButtonClass}
//               onClick={() => CloseManagePanel()}
//             >
//               <div
//                 className={closeButtonImageClass}
//                 style={{
//                   maskImage: "url(Media/Glyphs/Close.svg)",
//                 }}
//               ></div>
//             </button>
//           </div>
//         </div>
//         <div className={styleDefault.content}>
//           <PanelSection>
//             <PanelSectionRow
//               left={"SelectedEntityTitleText"}
//               right={"`${props.w_entity.index}:${props.w_entity.version}`"}
//             />
//             <PanelSectionRow
//               left={"CurrentBrandTitleText"}
//               right={"props.w_brand"}
//             />
//             <PanelSectionRow
//               left={"CurrentCompanyTitleText"}
//               right={"props.w_company"}
//             />
//           </PanelSection>
//         </div>
//       </div>
//     </Portal>
//   );
// };

export default {}