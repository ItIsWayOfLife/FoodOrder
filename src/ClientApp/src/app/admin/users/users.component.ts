import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';

import { RolesService } from '../../service/roles.service';
import { UserService } from '../../service/user.service';

import { User } from './user';
import { UserChangePassword } from './userChangePassword';
import { UserChangeRoles } from './userChangeRoles';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.css'],
  providers: [RolesService, UserService]
})
export class UsersComponent implements OnInit {

  isAmdim: boolean;

  editedUser: User;
  users: Array<User>;
  isView: boolean;
  isNewRecord: boolean;
  isEdit: boolean;

  isShowStatusMessage: boolean;
  statusMessage: string;
  isMessInfo: boolean;

  userChangePassword: UserChangePassword;
  isChangePasword: boolean;

  isChangeRoles: boolean;
  userChangeRoles: UserChangeRoles;

  allRoles: Array<string>;

  searchSelectionString: string;
  searchStr: string;

  constructor(private rolesServ: RolesService,
    private usersServ: UserService,
    private titleService: Title) {

    this.titleService.setTitle('Users');

    this.users = new Array<User>();
    this.isAmdim = this.rolesServ.isAdminRole();
    this.isView = true;
    this.isNewRecord = false;
    this.isShowStatusMessage = false;
    this.isEdit = false;

    this.userChangePassword = new UserChangePassword();
    this.isChangePasword = false;

    this.isChangeRoles = false;
    this.userChangeRoles = new UserChangeRoles();
    this.allRoles = new Array<string>();

    this.searchSelectionString = "Search by";
    this.searchStr = "";
  }

  ngOnInit(): void {
    this.loadUsers();
  }

  // load users
  loadUsers() {
    this.usersServ.getUsers().subscribe((data: User[]) => {
      this.users = data;
    });
  }

  // add user
  addUser() {
    this.editedUser = new User();
    this.users.push(this.editedUser);
    this.isNewRecord = true;
    this.isView = false;
  }

  // edit user
  editUser(user: User) {
    this.isEdit = true;
    this.isView = false;
    this.editedUser = user;
  }

  // save user
  saveUser() {
    if (this.isNewRecord) {
      // add user
      this.usersServ.createUser(this.editedUser).subscribe(response => {

        this.loadUsers();
        this.statusMessage = 'Data added successfully';
        this.isMessInfo = true;
        this.isShowStatusMessage = true;
      }, err => {
          this.users.pop();
          this.statusMessage = 'Error adding data';
          if (typeof err.error == 'string') {
            this.statusMessage += '. ' + err.error;
          }
          this.isMessInfo = false;
          this.isShowStatusMessage = true;
          console.log(err);
        }
      );
    } else {
      // edit user
      this.usersServ.updateUser(this.editedUser).subscribe(response => {

        this.loadUsers();
        this.statusMessage = 'Data changed successfully';
        this.isMessInfo = true;
        this.isShowStatusMessage = true;
      }, err => {
        this.loadUsers();
        this.statusMessage = 'Error while changing data';
        if (typeof err.error == 'string') {
          this.statusMessage += '. ' + err.error;
        }
        this.isMessInfo = false;
        this.isShowStatusMessage = true;
        console.log(err);
      });
    }
  }

  // cancel (back)
  cancel() {
    // if cancel for add, delete last user
    if (this.isNewRecord) {
      this.users.pop();
    }
    this.isNewRecord = false;
    this.isView = true;
    this.isEdit = false;
    this.isChangePasword = false;
    this.editedUser = null;
    this.isChangeRoles = false;
  }

  // delete user
  deleteUser(user: User) {
    this.usersServ.deleteUser(user.id).subscribe(response => {

      this.loadUsers();
      this.statusMessage = 'Data deleted successfully';
      this.isMessInfo = true;
      this.isShowStatusMessage = true;
    }, err => {
      this.statusMessage = 'Delete error';
      if (typeof err.error == 'string') {
        this.statusMessage += '. ' + err.error;
      }
      this.isMessInfo = false;
      this.isShowStatusMessage = true;
      console.log(err);
    });
  }

  // show info
  showStatusMess() {
    this.isShowStatusMessage = !this.isShowStatusMessage;
    this.isNewRecord = false;
    this.isView = true;
    this.isEdit = false;
    this.isChangeRoles = false;
    this.isChangePasword = false;
  }

  // edit pass get data
  changePassUser(id: string) {
    this.userChangePassword = new UserChangePassword();
    this.userChangePassword.id = id;
    this.isView = false;
    this.isChangePasword = true;
  }

  // edit pass
  editPassUser() {
    this.usersServ.changePass(this.userChangePassword).subscribe(response => {
      this.statusMessage = 'Password changed successfully';
      this.isMessInfo = true;
      this.isShowStatusMessage = true;
    }, err => {
      this.statusMessage = 'Password change error';
      if (typeof err.error == 'string') {
        this.statusMessage += '. ' + err.error;
      }
      this.isMessInfo = false;
      this.isShowStatusMessage = true;
      console.log(err);
    });
  }

  changeRolesUser(id: string) {
    this.userChangeRoles = new UserChangeRoles();

    this.userChangeRoles.roles = new Array<string>();
    this.allRoles = new Array<string>();
    this.userChangeRoles.id = id;

    this.isView = false;
    this.isChangeRoles = true;

    this.rolesServ.getUserRoles(id).subscribe((data: string[]) => {
      this.userChangeRoles.roles = data;
    });

    this.rolesServ.getRoles().subscribe((data: string[]) => {
      this.allRoles = data;
    });
  }

  checkRole(role: string): boolean {
    if (this.userChangeRoles.roles.includes(role)) {
      return true;
    }
    else {
      return false;
    }
  }

  editUserRoles() {
    this.rolesServ.editRoles(this.userChangeRoles).subscribe(response => {
      this.statusMessage = 'Access rights changed successfully';
      this.isMessInfo = true;
      this.isShowStatusMessage = true;
    }, err => {
      this.statusMessage = 'Error when changing access rights';
      if (typeof err.error == 'string') {
        this.statusMessage += '. ' + err.error;
      }
      this.isMessInfo = false;
      this.isShowStatusMessage = true;
      console.log(err);
    });
  }

  pressRole(role: string) {
    if (!this.userChangeRoles.roles.includes(role)) {
      this.userChangeRoles.roles.push(role);
    }
    else {
      const index: number = this.userChangeRoles.roles.indexOf(role);
      if (index !== -1) {
        this.userChangeRoles.roles.splice(index, 1);
      }
    }
  }

  getViewUsers(): Array<User> {
    if (this.searchSelectionString == "Id") {
      return this.users.filter(x => x.id.toLowerCase().includes(this.searchStr.toLowerCase()));
    }
    else if (this.searchSelectionString == "Email") {
      return this.users.filter(x => x.email.toLowerCase().includes(this.searchStr.toLowerCase()));
    }
    else if (this.searchSelectionString == "FLP") {
      let selectUserByFLP: Array<User> = new Array<User>();

      for (let user of this.users) {
        let flp: string = user.firstname + " " + user.lastname + " " + user.patronymic;
        if (flp.toLowerCase().includes(this.searchStr.toLowerCase())) {
          selectUserByFLP.push(user);
        }
      }
      return selectUserByFLP;
    }

    return this.users;
  }

  refresh() {
    this.searchSelectionString = "Search by";
    this.searchStr = "";
  }

}
