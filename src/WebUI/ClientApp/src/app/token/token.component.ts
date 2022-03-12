import { Component, OnInit } from "@angular/core";
import { AuthorizeService } from "../../api-authorization/authorize.service";

import { faCopy } from "@fortawesome/free-solid-svg-icons";

@Component({
  selector: "app-token-component",
  templateUrl: "./token.component.html",
})
export class TokenComponent implements OnInit {
  token: string;
  isError: boolean;
  isCopied: boolean;

  faCopy = faCopy;

  constructor(private authorizeService: AuthorizeService) {}

  ngOnInit(): void {
    this.isCopied = false;
    this.authorizeService.getAccessToken().subscribe(
      (t) => {
        this.token = "Bearer " + t;
        this.isError = false;
      },
      (err) => {
        this.isError = true;
      }
    );
  }

  copyToClipboard(): void {
    const selBox = document.createElement("textarea");
    selBox.style.position = "fixed";
    selBox.style.left = "0";
    selBox.style.top = "0";
    selBox.style.opacity = "0";
    selBox.value = this.token;
    document.body.appendChild(selBox);
    selBox.focus();
    selBox.select();
    document.execCommand("copy");
    document.body.removeChild(selBox);
    this.isCopied = true;
  }
}
