import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';

import { AccountService } from '../../service/account.service';
import { RolesService } from '../../service/roles.service';

import { ProfileModel } from './profileModel';
import { ChangePasswordModel } from './changePasswordModel';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css'],
  providers: [AccountService,
    RolesService]
})
export class ProfileComponent implements OnInit {

  _firstname: string;
  _lastname: string;
  _patronymic: string;
  _email: string;

  editProfileModel: ProfileModel;

  isEditProfile: boolean;
  isStatusEditGood: boolean;
  isStatusEditBad: boolean;

  changePasswordModel: ChangePasswordModel;

  isEditPass: boolean;
  isStatusEditPassGood: boolean;
  isStatusEditPassBad: boolean;

  isViewMyRole: boolean;

  constructor(private serv: AccountService,
    private rolesServ: RolesService,
    private titleService: Title) {

    this.titleService.setTitle('Profile');
    this.editProfileModel = new ProfileModel();

    this.isEditProfile = false;
    this.isStatusEditGood = false;
    this.isStatusEditBad = false;

    this.changePasswordModel = new ChangePasswordModel();

    this.isEditPass = false;
    this.isStatusEditPassGood = false;
    this.isStatusEditPassBad = false;

    this.isViewMyRole = false;
  }

  getData(data: ProfileModel) {
    this._email = data.email;
    this._firstname = data.firstname;
    this._lastname = data.lastname;
    this._patronymic = data.patronymic;
  }

  ngOnInit(): void {
    this.serv.getProfile().subscribe((data: ProfileModel) => {
      this.getData(data);
      this.editProfileModel = data;
    },
      err => {
        console.log(err);
      });
  }

  getMyRoles() {
    return this.rolesServ.myGetRelos();
  }

  editProfileShow() {
    this.isEditProfile = !this.isEditProfile;

    this.isEditPass = false;
    this.isViewMyRole = false;
  }

  editProfile() {
    this.serv.editProfile(this.editProfileModel).subscribe(response => {

      if (response.status == 200) {
        this.isStatusEditGood = true;

        this.getData(this.editProfileModel);

        this.isStatusEditBad = false;
      }
      else {
        this.isStatusEditBad = true;
      }
    }, err => {
      console.log(err);
      this.isStatusEditBad = true;
    });
  }

  okEdit() {
    this.isStatusEditGood = !this.isStatusEditGood;
  }

  editPassShow() {
    this.isEditPass = !this.isEditPass;

    this.isEditProfile = false;
    this.isViewMyRole = false;
  }

  okEditPass() {
    this.isStatusEditPassGood = !this.isStatusEditPassGood;
  }

  editPass() {
    this.serv.editPass(this.changePasswordModel).subscribe(response => {
      if (response.status == 200) {
        this.isStatusEditPassGood = true;
        this.isStatusEditPassBad = false;
      }
      else {
        this.isStatusEditPassBad = true;
      }
    }, err => {
      console.log(err);
      this.isStatusEditPassBad = true;
    });
  }

  viewRole() {
    this.isViewMyRole = !this.isViewMyRole;

    this.isEditProfile = false;
    this.isEditPass = false;
  }

}
