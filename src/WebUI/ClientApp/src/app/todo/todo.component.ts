import { Component } from '@angular/core';
import { TodoItemsClient, TodoItemsListVm, CreateTodoItemCommand, TodoItemDto, UpdateTodoItemCommand } from '../cleanarchitecture-api';
import { faTimes } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-todo-component',
  templateUrl: './todo.component.html',
  styleUrls: ['./todo.component.css']
})
export class TodoComponent {
  public vm: TodoItemsListVm;
  public newTodo: string = "";

  faTimes = faTimes;

  constructor(private client: TodoItemsClient) {
    client.getAll().subscribe(
      result => this.vm = result,
      error => console.error(error)
    );
  }

  addTodo() {
    var command = CreateTodoItemCommand.fromJS({ name: this.newTodo });

    this.client.create(command).subscribe(
      result => {
        this.vm.todoItems.push(
          TodoItemDto.fromJS(
            {
              id: result,
              name: this.newTodo,
              isComplete: false
            }));

        this.newTodo = '';
      },
      error => console.error(error)
    );
  }

  updateTodo(item: TodoItemDto) {
    this.client.update(item.id, UpdateTodoItemCommand.fromJS(item))
      .subscribe(
        () => console.log('Update succeeded.'),
        error => console.error(error)
      );
  }

  deleteTodo(id: number) {
    this.client.delete(id).subscribe(
      () => this.vm.todoItems = this.vm.todoItems.filter(t => t.id != id),
      error => console.error(error)
    );
  }

  remainingItems(): number {
    return this.vm.todoItems.filter(t => !t.isComplete).length
  }
}
