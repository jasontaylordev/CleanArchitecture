import { Component, OnInit, TemplateRef, signal, computed } from '@angular/core';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { TodoListsClient, TodoItemsClient,
  TodoListDto, TodoItemDto, LookupDto,
  CreateTodoListCommand, UpdateTodoListCommand,
  CreateTodoItemCommand, UpdateTodoItemCommand, UpdateTodoItemDetailCommand
} from '../web-api-client';

@Component({
  standalone: false,
  selector: 'app-todo-component',
  templateUrl: './todo.component.html',
  styleUrls: ['./todo.component.scss']
})
export class TodoComponent implements OnInit {
  debug = false;
  lists = signal<TodoListDto[] | null>(null);
  priorityLevels = signal<LookupDto[]>([]);
  selectedListId = signal<number | null>(null);
  selectedList = computed(() => this.lists()?.find(l => l.id === this.selectedListId()) ?? null);
  selectedItem = signal<TodoItemDto | null>(null);
  newListEditor: any = {};
  newListError = signal('');
  listOptionsEditor: any = {};
  itemDetailsEditor: any = {};
  newListModalRef: BsModalRef;
  listOptionsModalRef: BsModalRef;
  deleteListModalRef: BsModalRef;
  itemDetailsModalRef: BsModalRef;

  constructor(
    private listsClient: TodoListsClient,
    private itemsClient: TodoItemsClient,
    private modalService: BsModalService
  ) {}

  ngOnInit(): void {
    this.listsClient.getTodoLists().subscribe({
      next: result => {
        this.lists.set(result.lists);
        this.priorityLevels.set(result.priorityLevels);
        if (result.lists.length) {
          this.selectedListId.set(result.lists[0].id);
        }
      },
      error: error => console.error(error)
    });
  }

  // Lists
  remainingItems(list: TodoListDto): number {
    return list.items.filter(t => !t.done).length;
  }

  showNewListModal(template: TemplateRef<any>): void {
    this.newListModalRef = this.modalService.show(template);
    setTimeout(() => document.getElementById('title').focus(), 250);
  }

  newListCancelled(): void {
    this.newListModalRef.hide();
    this.newListEditor = {};
    this.newListError.set('');
  }

  addList(): void {
    const list = {
      id: 0,
      title: this.newListEditor.title,
      items: []
    } as TodoListDto;

    this.listsClient.createTodoList(list as CreateTodoListCommand).subscribe({
      next: result => {
        list.id = result;
        this.lists.update(ls => [...ls, list]);
        this.selectedListId.set(list.id);
        this.newListModalRef.hide();
        this.newListEditor = {};
        this.newListError.set('');
      },
      error: error => {
        const errors = JSON.parse(error.response).errors;
        if (errors && errors.Title) {
          this.newListError.set(errors.Title[0]);
        }
        setTimeout(() => document.getElementById('title').focus(), 250);
      }
    });
  }

  showListOptionsModal(template: TemplateRef<any>) {
    this.listOptionsEditor = {
      id: this.selectedList()!.id,
      title: this.selectedList()!.title
    };
    this.listOptionsModalRef = this.modalService.show(template);
  }

  updateListOptions() {
    const id = this.selectedList()!.id;
    const newTitle = this.listOptionsEditor.title;
    this.listsClient.updateTodoList(id, this.listOptionsEditor as UpdateTodoListCommand).subscribe({
      next: () => {
        this.lists.update(ls => ls.map(l => l.id === id ? { ...l, title: newTitle } as TodoListDto : l));
        this.listOptionsModalRef.hide();
        this.listOptionsEditor = {};
      },
      error: error => console.error(error)
    });
  }

  confirmDeleteList(template: TemplateRef<any>) {
    this.listOptionsModalRef.hide();
    this.deleteListModalRef = this.modalService.show(template);
  }

  deleteListConfirmed(): void {
    const deletedId = this.selectedList()!.id;
    this.listsClient.deleteTodoList(deletedId).subscribe({
      next: () => {
        this.deleteListModalRef.hide();
        this.lists.update(ls => ls.filter(l => l.id !== deletedId));
        const remaining = this.lists()!;
        this.selectedListId.set(remaining.length ? remaining[0].id : null);
      },
      error: error => console.error(error)
    });
  }

  // Items
  showItemDetailsModal(template: TemplateRef<any>, item: TodoItemDto): void {
    this.selectedItem.set(item);
    this.itemDetailsEditor = { ...item };
    this.itemDetailsModalRef = this.modalService.show(template);
  }

  updateItemDetails(): void {
    const currentItem = this.selectedItem()!;
    const isMoving = currentItem.listId !== this.itemDetailsEditor.listId;
    this.itemsClient.updateTodoItemDetail(currentItem.id, this.itemDetailsEditor as UpdateTodoItemDetailCommand).subscribe({
      next: () => {
        this.lists.update(ls => ls.map(l => {
          if (l.id === currentItem.listId && isMoving) {
            return { ...l, items: l.items.filter(i => i.id !== currentItem.id) } as TodoListDto;
          }
          if (l.id === this.itemDetailsEditor.listId && isMoving) {
            const moved = { ...currentItem, listId: this.itemDetailsEditor.listId, priority: this.itemDetailsEditor.priority, note: this.itemDetailsEditor.note } as TodoItemDto;
            return { ...l, items: [...l.items, moved] } as TodoListDto;
          }
          if (l.id === currentItem.listId) {
            return { ...l, items: l.items.map(i => i.id === currentItem.id
              ? { ...i, priority: this.itemDetailsEditor.priority, note: this.itemDetailsEditor.note } as TodoItemDto
              : i
            )} as TodoListDto;
          }
          return l;
        }));
        this.selectedItem.set(null);
        this.itemDetailsModalRef.hide();
        this.itemDetailsEditor = {};
      },
      error: error => console.error(error)
    });
  }

  addItem() {
    const item = {
      id: 0,
      listId: this.selectedList()!.id,
      priority: this.priorityLevels()[0].id,
      title: '',
      done: false
    } as TodoItemDto;

    const index = this.selectedList()!.items.length;
    this.lists.update(ls => ls.map(l => l.id === this.selectedListId()
      ? { ...l, items: [...l.items, item] } as TodoListDto
      : l
    ));
    this.editItem(item, 'itemTitle' + index);
  }

  editItem(item: TodoItemDto, inputId: string): void {
    this.selectedItem.set(item);
    setTimeout(() => document.getElementById(inputId).focus(), 100);
  }

  updateItem(item: TodoItemDto, pressedEnter: boolean = false): void {
    const isNewItem = item.id === 0;

    if (!item.title.trim()) {
      this.deleteItem(item);
      return;
    }

    if (item.id === 0) {
      const listId = this.selectedListId()!;
      this.itemsClient
        .createTodoItem({ title: item.title, listId } as CreateTodoItemCommand)
        .subscribe({
          next: result => {
            this.lists.update(ls => ls.map(l => l.id === listId
              ? { ...l, items: l.items.map(i => i === item ? { ...i, id: result } as TodoItemDto : i) } as TodoListDto
              : l
            ));
          },
          error: error => console.error(error)
        });
    } else {
      this.itemsClient.updateTodoItem(item.id, item as UpdateTodoItemCommand).subscribe({
        next: () => console.log('Update succeeded.'),
        error: error => console.error(error)
      });
    }

    this.selectedItem.set(null);

    if (isNewItem && pressedEnter) {
      setTimeout(() => this.addItem(), 250);
    }
  }

  deleteItem(item: TodoItemDto) {
    if (this.itemDetailsModalRef) {
      this.itemDetailsModalRef.hide();
    }

    const listId = this.selectedListId()!;
    if (item.id === 0) {
      const currentItem = this.selectedItem()!;
      this.lists.update(ls => ls.map(l => l.id === listId
        ? { ...l, items: l.items.filter(i => i !== currentItem) } as TodoListDto
        : l
      ));
      this.selectedItem.set(null);
    } else {
      this.itemsClient.deleteTodoItem(item.id).subscribe({
        next: () => {
          this.lists.update(ls => ls.map(l => l.id === listId
            ? { ...l, items: l.items.filter(i => i.id !== item.id) } as TodoListDto
            : l
          ));
        },
        error: error => console.error(error)
      });
    }
  }
}
