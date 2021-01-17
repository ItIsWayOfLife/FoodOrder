import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { LoginModel } from '../account/login/LoginModel';
import { RegisterModel } from '../account/register/registerModel';
import { ProfileModel } from '../account/profile/profileModel';
import { ChangePasswordModel } from '../account/profile/changePasswordModel';

@Injectable()
export class AccountService {

  private url = "https://localhost:44342/api/account";

  constructor(private http: HttpClient) { }

  login(model: LoginModel) {
    const myHeaders = new HttpHeaders().set("Content-Type", "application/json");
    let credentials = JSON.stringify(model);
    return this.http.post(this.url +"/login", credentials,
     { headers: myHeaders });
  }

  register(model: RegisterModel) {
    const myHeaders = new HttpHeaders().set("Content-Type", "application/json");
    let credentials = JSON.stringify(model);
    return this.http.post(this.url +"/register", credentials,
      { headers: myHeaders, observe: 'response' });
  }

  getProfile() {
    return this.http.get(this.url + "/profile");
  }

  editProfile(model: ProfileModel) {
    const myHeaders = new HttpHeaders().set("Content-Type", "application/json");
    let credentials = JSON.stringify(model);
    return this.http.put(this.url + "/editprofile", credentials,
      { headers: myHeaders, observe: 'response' });
  }

  editPass(model: ChangePasswordModel) {
    const myHeaders = new HttpHeaders().set("Content-Type", "application/json");
    let credentials = JSON.stringify(model);
    return this.http.put(this.url + "/changepassword", credentials,
      { headers: myHeaders, observe: 'response' });
  }
}
