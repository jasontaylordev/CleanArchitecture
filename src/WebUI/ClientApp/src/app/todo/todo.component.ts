import { Component, TemplateRef } from '@angular/core';
import { TodoItemsClient, CreateTodoItemCommand, TodoItemDto, UpdateTodoItemCommand, TodosVm, TodoListsClient, TodoListDto, CreateTodoListCommand, UpdateTodoListCommand, UpdateTodoItemDetailCommand } from '../cleanarchitecture-api';
import { faPlus, faEllipsisH } from '@fortawesome/free-solid-svg-icons';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';

@Component({
    selector: 'app-todo-component',
    templateUrl: './todo.component.html',
    styleUrls: ['./todo.component.css']
})
export class TodoComponent {

    debug: boolean;

    vm: TodosVm;

    selectedList: TodoListDto;
    selectedItem: TodoItemDto;
    
    newListEditor: any = {};
    listOptionsEditor: any = {};
    itemDetailsEditor: any = {};

    newListModalRef: BsModalRef;
    listOptionsModalRef: BsModalRef;
    deleteListModalRef: BsModalRef;
    itemDetailsModalRef: BsModalRef;

    faPlus = faPlus;
    faEllipsisH = faEllipsisH;

    constructor(private listsClient: TodoListsClient, private itemsClient: TodoItemsClient, private modalService: BsModalService) {
        listsClient.get().subscribe(
            result => {
                this.vm = result;
                if (this.vm.lists.length) {
                    this.selectedList = this.vm.lists[0];
                }
            },
            error => console.error(error)
        );
    }

    // Lists
    remainingItems(list: TodoListDto): number {
        return list.items.filter(t => !t.done).length;
    }

    showNewListModal(template: TemplateRef<any>): void {
        this.newListModalRef = this.modalService.show(template);
        setTimeout(() => document.getElementById("title").focus(), 250);
    }
    
    newListCancelled(): void {
        this.newListModalRef.hide();
        this.newListEditor = {};
    }

    addList(): void {
        let list = TodoListDto.fromJS({
            id: 0,
            title: this.newListEditor.title,
            items: [],
        });

        this.listsClient.create(<CreateTodoListCommand>{ title: this.newListEditor.title }).subscribe(
            result => {
                list.id = result;
                this.vm.lists.push(list);
                this.selectedList = list;
                this.newListModalRef.hide();
                this.newListEditor = {};
            },
            error => {
                let errors = JSON.parse(error.response);

                if (errors && errors.Title) {
                    this.newListEditor.error = errors.Title[0];
                }

                setTimeout(() => document.getElementById("title").focus(), 250);
            }
        );
    }

    showListOptionsModal(template: TemplateRef<any>) {
        this.listOptionsEditor = {
            id: this.selectedList.id,
            title: this.selectedList.title,
        };

        this.listOptionsModalRef = this.modalService.show(template);
    }

    updateListOptions() {
        this.listsClient.update(this.selectedList.id, UpdateTodoListCommand.fromJS(this.listOptionsEditor))
            .subscribe(
                () => {
                    this.selectedList.title = this.listOptionsEditor.title,
                    this.listOptionsModalRef.hide();
                    this.listOptionsEditor = {};
                },
                error => console.error(error)
            );
    }

    confirmDeleteList(template: TemplateRef<any>) {
        this.listOptionsModalRef.hide();
        this.deleteListModalRef = this.modalService.show(template);
    }

    deleteListConfirmed(): void {
        this.listsClient.delete(this.selectedList.id).subscribe(
            () => {
                this.deleteListModalRef.hide();
                this.vm.lists = this.vm.lists.filter(t => t.id != this.selectedList.id)
                this.selectedList = this.vm.lists.length ? this.vm.lists[0] : null;
            },
            error => console.error(error)
        );
    }

    // Items

    showItemDetailsModal(template: TemplateRef<any>, item: TodoItemDto): void {
        this.selectedItem = item;
        this.itemDetailsEditor = {
            ...this.selectedItem
        };

        this.itemDetailsModalRef = this.modalService.show(template);
    }

    updateItemDetails(): void {
        this.itemsClient.updateItemDetails(this.selectedItem.id, UpdateTodoItemDetailCommand.fromJS(this.itemDetailsEditor))
            .subscribe(
                () => {
                    if (this.selectedItem.listId != this.itemDetailsEditor.listId) {
                        this.selectedList.items = this.selectedList.items.filter(i => i.id != this.selectedItem.id)
                        let listIndex = this.vm.lists.findIndex(l => l.id == this.itemDetailsEditor.listId);
                        this.selectedItem.listId = this.itemDetailsEditor.listId;
                        this.vm.lists[listIndex].items.push(this.selectedItem);
                    }

                    this.selectedItem.priority = this.itemDetailsEditor.priority;
                    this.selectedItem.note = this.itemDetailsEditor.note;
                    this.itemDetailsModalRef.hide();
                    this.itemDetailsEditor = {};
                },
                error => console.error(error)
            );
    }

    addItem() {
        let item = TodoItemDto.fromJS({
            id: 0,
            listId: this.selectedList.id,
            priority: this.vm.priorityLevels[0].value,
            title: '',
            done: false
        });

        this.selectedList.items.push(item);
        let index = this.selectedList.items.length - 1;
        this.editItem(item, 'itemTitle' + index);
    }

    editItem(item: TodoItemDto, inputId: string): void {
        this.selectedItem = item;
        setTimeout(() => document.getElementById(inputId).focus(), 100);
    }

    updateItem(item: TodoItemDto, pressedEnter: boolean = false): void {
        let isNewItem = item.id == 0;

        if (!item.title.trim()) {
            this.deleteItem(item);
            return;
        }

        if (item.id == 0) {
            this.itemsClient.create(CreateTodoItemCommand.fromJS({ ...item, listId: this.selectedList.id }))
                .subscribe(
                    result => {
                        item.id = result;
                    },
                    error => console.error(error)
                );
        } else {
            this.itemsClient.update(item.id, UpdateTodoItemCommand.fromJS(item))
                .subscribe(
                    () => console.log('Update succeeded.'),
                    error => console.error(error)
                );
        }

        this.selectedItem = null;

        if (isNewItem && pressedEnter) {
            this.addItem();
        }
    }

    // Delete item
    deleteItem(item: TodoItemDto) {
        if (this.itemDetailsModalRef) {
            this.itemDetailsModalRef.hide();
        }

        if (item.id == 0) {
            let itemIndex = this.selectedList.items.indexOf(this.selectedItem);
            this.selectedList.items.splice(itemIndex, 1);
        } else {
            this.itemsClient.delete(item.id).subscribe(
                () => this.selectedList.items = this.selectedList.items.filter(t => t.id != item.id),
                error => console.error(error)
            );
        }
    }
}
