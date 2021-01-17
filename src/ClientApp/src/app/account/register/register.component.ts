import { Component, OnInit } from '@angular/core';
import { Router } from "@angular/router";
import { Title } from '@angular/platform-browser';

import { AccountService } from '../../service/account.service';

import { RegisterModel } from './registerModel';
import { LoginModel } from '../login/LoginModel';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
  providers: [AccountService]
})
export class RegisterComponent implements OnInit {

  statusCode: number;
  statusOk: boolean;
  statusNotOk: boolean;

  isShowStatusMessage: boolean;
  statusMessage: string;

  invalidLogin: boolean;
  registerModel: RegisterModel;
  newLogModel: LoginModel;

  constructor(private router: Router,
    private serv: AccountService,
    private titleService: Title) {

    this.titleService.setTitle('Register');

    this.registerModel = new RegisterModel();
    this.statusOk = false;
    this.statusNotOk = false;

    this.isShowStatusMessage = false;
  }

  ngOnInit(): void {
  }

  register() {
    this.serv.register(this.registerModel)
      .subscribe(response => {

        this.statusCode = response.status;

        if (response.status == 200) {
          this.statusOk = true;

          this.newLogModel = new LoginModel();
          this.newLogModel.email = this.registerModel.email;
          this.newLogModel.password = this.registerModel.password;
          this.serv.login(this.newLogModel).subscribe(response => {
            let token = (<any>response).token;
            localStorage.setItem("jwt", token);
            this.invalidLogin = false;

            this.statusMessage = "registration completed successfully";
            this.isShowStatusMessage = true;
          }, err => {
            this.invalidLogin = true;
            console.log(err.message);
          });;
        }
        else {
          this.statusOk = false;
        }
        console.log("Code: " + response.status);
      },
        err => {
          this.statusNotOk = true;
          console.log("Code: " + err.status);
          console.log(err.message);

          this.statusMessage = "registration error";
          this.isShowStatusMessage = true;
        });
  }

  ok() {
    this.router.navigate(["/"]);
  }

  // show info
  showStatusMess() {
    this.isShowStatusMessage = !this.isShowStatusMessage;
  }

  comparePass(){
    if (this.registerModel.password==this.registerModel.passwordConfirm){
      return true;
    }
    else{
      return false;
    }
  }
}
