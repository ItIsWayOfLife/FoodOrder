import { Component, OnInit,   DoCheck } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';

import {RolesService} from '../../service/roles.service'

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css'],
  providers:[RolesService]
})
export class HeaderComponent implements OnInit,  DoCheck {

  isAmdim:boolean;
  isAdminOrEmployeeMyRole:boolean;

  constructor(private jwtHelper: JwtHelperService, 
    private rolesServ: RolesService) {  }

  logOut() {
    localStorage.removeItem("jwt");
 }

  isUserAuthenticated() {
    let token: string = localStorage.getItem("jwt");
    
    if (token && !this.jwtHelper.isTokenExpired(token)) {
      return true;
    }
    else {
      return false;
    }
  }

  ngOnInit(): void {
   this.getRoles();
  }

  ngDoCheck() {
    this.getRoles(); 
  }

  getRoles(){
    this.isAdminOrEmployeeMyRole = this.rolesServ.isAdminOrEmployeeRole();
    this.isAmdim = this.rolesServ.isAdminRole();
  }

}
