import { Injectable} from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { UserChangeRoles } from '../admin/users/userChangeRoles';

@Injectable()
export class RolesService{

   private url = "https://localhost:44342/api/roles";

   constructor(private http: HttpClient) {}

   isAdminRole(): boolean {
      if (this.myGetRelos() != null) {
         if (this.myGetRelos().includes("admin")) {
            return true;
         }
         else {
            return false;
         }
      }
      else {
         return false;
      }
   }

   isEmployeeRole(): boolean {
      if (this.myGetRelos() != null) {
         if (this.myGetRelos().includes("employee")) {
            return true;
         }
         else {
            return false;
         }
      }
      else {
         return false;
      }
   }

   isAdminOrEmployeeRole(): boolean {
      if (this.isAdminRole() || this.isEmployeeRole()) {
         return true;
      }
      else {
         return false;
      }
   }

   myGetRelos(): Array<string> {
      let token: string = localStorage.getItem("jwt");

      if (token == null)
         return null;

      let arrayRoles: Array<string> = new Array<string>();

      let jwtData = token.split('.')[1];
      let decodedJwtJsonData = window.atob(jwtData);


      if (decodedJwtJsonData.includes("admin")) {
         arrayRoles.push("admin");
      }

      if (decodedJwtJsonData.includes("employee")) {
         arrayRoles.push("employee");
      }
      return arrayRoles;
   }

   getRoles() {
      return this.http.get(this.url);
   }

   getUserRoles(id: string) {
      return this.http.get(this.url + '/' + id);
   }

   editRoles(userChangeRoles: UserChangeRoles) {
      const myHeaders = new HttpHeaders().set("Content-Type", "application/json");
      return this.http.put(this.url, JSON.stringify(userChangeRoles), { headers: myHeaders, observe: 'response' });
   }
}
