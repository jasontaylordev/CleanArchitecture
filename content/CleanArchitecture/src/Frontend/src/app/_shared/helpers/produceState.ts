import { patchState, Prettify, WritableStateSource } from "@ngrx/signals";
import { Draft, produce } from "immer";

export function produceState<State extends object>(
  store: WritableStateSource<State>,
  updater: (state: Draft<Prettify<State>>) => Draft<State> | void,
) {
  patchState(store, state => produce(state, updater));
}
