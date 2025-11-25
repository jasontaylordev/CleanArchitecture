import { isDevMode } from "@angular/core";
import {
  getState,
  patchState,
  signalStoreFeature,
  withHooks,
  withMethods,
} from "@ngrx/signals";
interface ImportMeta {
  hot?: {
    on(
      event: "vite:beforeFullReload",
      callback: (info: { type: string; path: string }) => void,
    ): void;
  };
}

const STORE_NAME = "ct-dev-reload";
const STORE_VERSION = 1;

export function withDevReloadHelper(key: string, ...ignoreKeys: string[]) {
  if (!isDevMode() || !(import.meta as ImportMeta).hot) {
    return signalStoreFeature(withMethods(() => ({})));
  }

  return signalStoreFeature(
    withHooks({
      async onInit(store) {
        (import.meta as ImportMeta).hot?.on("vite:beforeFullReload", () =>
          storeState(key, getState(store), ignoreKeys),
        );

        const state = await loadState(key);
        if (state) {
          patchState(store, state);
          console.log("Restored state from hot reload: ", key, state);
        }
      },
    }),
  );
}

async function storeState(
  key: string,
  state: unknown,
  ignoreKeys: string[],
): Promise<void> {
  const transaction = await openDB();
  const store = transaction.objectStore("states");
  for (const key of ignoreKeys) {
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    delete (state as any)[key];
  }
  store.put({ key, state });
  await indexedDBPromise(transaction);
}

async function loadState(key: string): Promise<unknown> {
  const transaction = await openDB();
  const store = transaction.objectStore("states");
  const request = store.get(key) as IDBRequest<
    { key: string; state: unknown } | undefined
  >;
  await indexedDBPromise(request);
  if (!request.result) {
    return undefined;
  }

  const result = request.result.state;

  await indexedDBPromise(store.delete(key));
  await indexedDBPromise(transaction);

  return result;
}

async function openDB(): Promise<IDBTransaction> {
  const request = indexedDB.open(STORE_NAME, STORE_VERSION);

  request.onupgradeneeded = () => {
    const db = request.result;
    if (!db.objectStoreNames.contains("states")) {
      db.createObjectStore("states", { keyPath: "key" });
    }
  };

  await indexedDBPromise(request);
  return request.result.transaction("states", "readwrite");
}

function indexedDBPromise(
  element: IDBOpenDBRequest | IDBTransaction | IDBRequest,
): Promise<void> {
  return new Promise((resolve, reject) => {
    if ("onsuccess" in element) {
      element.onsuccess = () => resolve();
    } else if ("oncomplete" in element) {
      element.oncomplete = () => resolve();
    }
    element.onerror = () => reject(element.error);
  });
}
