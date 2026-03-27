import { Component } from "@angular/core";
import { RouterModule } from "@angular/router";

@Component({
  selector: "app-navbar",
  standalone: true,
  imports: [RouterModule],
  templateUrl: "./navbar.html",
  styleUrl: "./navbar.css",
})
export class NavbarComponent {}
