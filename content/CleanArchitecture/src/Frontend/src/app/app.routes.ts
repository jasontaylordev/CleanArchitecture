import { Routes } from "@angular/router";

export const routes: Routes = [
  { path: "", redirectTo: "requests", pathMatch: "full" },
  {
    path: "requests",
    loadChildren: () =>
      import("./requests/requests.routes").then(
        (routes) => routes.requestsRoutes,
      ),
  },
];
