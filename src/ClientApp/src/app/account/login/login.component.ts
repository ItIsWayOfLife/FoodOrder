import { Component } from '@angular/core';
import { Router } from "@angular/router";
import { Title } from '@angular/platform-browser';

import {LoginModel} from './LoginModel';
import {AccountService} from '../../service/account.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
  providers: [AccountService]
})
export class LoginComponent {
  invalidLogin: boolean;

  loginModel:LoginModel;

  constructor(private router: Router,
     private serv: AccountService,
    private titleService: Title) {

      this.titleService.setTitle('Login');
    this.loginModel = new LoginModel();
  }

  login() {
  this.serv.login(this.loginModel)
    .subscribe(response => {
      let token = (<any>response).token;
      localStorage.setItem("jwt", token);
      this.invalidLogin = false;
      this.router.navigate([""]);
    }, err => {
      this.invalidLogin = true;
    });
  }
}

