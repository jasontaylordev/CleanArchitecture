import { signalStoreFeature, withMethods } from "@ngrx/signals";
import type { withDevReloadHelper as withDevReloadHelperType } from "./dev-reload-helper.development";

export const withDevReloadHelper: typeof withDevReloadHelperType = () => {
  return signalStoreFeature(withMethods(() => ({})));
};
