import { inject } from "@angular/core";
import { signalStore, withMethods, withState } from "@ngrx/signals";
import { firstValueFrom } from "rxjs";
import { withDevReloadHelper } from "../../../_shared/helpers/dev-reload-helper";
import { produceState } from "../../../_shared/helpers/produceState";
import {
  PaginatedListOfTodoItemBriefDto,
  TodoItemsClient,
} from "../../../_shared/services/api.service";

interface RequestListState {
  items: PaginatedListOfTodoItemBriefDto;
}

const initialState: RequestListState = {
  items: {
    items: [],
    pageNumber: 0,
    totalPages: 0,
    totalCount: 0,
    hasNextPage: false,
    hasPreviousPage: false,
  },
};

export const RequestListStore = signalStore(
  withState(initialState),
  withMethods((store, client = inject(TodoItemsClient)) => ({
    async loadItems(listId: number, pageNumber: number, pageSize: number) {
      const result = await firstValueFrom(
        client.getTodoItemsWithPagination(listId, pageNumber, pageSize),
      );
      produceState(store, (state) => {
        state.items = result;
      });
    },
  })),
  withDevReloadHelper("RequestListStore"),
);
