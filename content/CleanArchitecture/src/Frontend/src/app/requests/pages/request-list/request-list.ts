import { Component, inject, OnInit } from "@angular/core";
import { RequestListStore } from "./request-list.store";

@Component({
  selector: "app-request-list",
  standalone: true,
  imports: [],
  templateUrl: "./request-list.html",
  styleUrl: "./request-list.css",
  providers: [RequestListStore],
})
export class RequestListComponent implements OnInit {
  private readonly store = inject(RequestListStore);
  protected readonly items = this.store.items;

  ngOnInit(): void {
    this.store.loadItems(1, 1, 10);
  }
}
