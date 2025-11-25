import { Component, signal } from "@angular/core";
import { RouterOutlet } from "@angular/router";
import { NavbarComponent } from "./_root/components/navbar/navbar";

@Component({
  selector: "app-root",
  imports: [RouterOutlet, NavbarComponent],
  templateUrl: "./app.html",
  styleUrl: "./app.css",
})
export class App {
  protected readonly title = signal("Cubido.Template.WebApp");
}
