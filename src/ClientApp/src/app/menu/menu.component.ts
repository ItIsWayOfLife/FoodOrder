import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';
import { Title } from '@angular/platform-browser';

import { MenuService } from '../service/menu.service';
import { RolesService } from '../service/roles.service';
import { ProviderService } from '../service/provider.service';

import { Provider } from '../provider/provider';
import { Menu } from './menu';

@Component({
  selector: 'app-menu',
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.css'],
  providers: [RolesService,
    MenuService,
    ProviderService]
})
export class MenuComponent implements OnInit {

  providerId: number;

  isAdminMyRole: boolean;

  editedMenu: Menu;
  menus: Array<Menu>;

  isShowStatusMessage: boolean;
  statusMessage: string;
  isMessInfo: boolean;

  nameProvider: string;

  searchSelectionString: string;
  searchStr: string;

  editedDate: string;
  editOldDate: string;

  isEdit: boolean;
  isView: boolean;
  isNewRecord: boolean;

  constructor(private activateRoute: ActivatedRoute,
    private menuServ: MenuService,
    private rolesServ: RolesService,
    private providerServ: ProviderService,
    private _location: Location,
    private titleService: Title) {

    this.titleService.setTitle('Menu');

    this.providerId = activateRoute.snapshot.params['providerId'];

    this.menus = new Array<Menu>();
    this.editedMenu = new Menu();

    this.isShowStatusMessage = false;

    this.searchSelectionString = "Search by";
    this.searchStr = "";

    this.isView = true;
    this.isEdit = false;
    this.isNewRecord = false;

    this.editedDate = "";
  }

  backClicked() {
    this._location.back();
  }

  getEditDate() {
    let eDateMenu: string = this.editedMenu.date;
    this.editOldDate = this.editedMenu.date;
    let eDateDay = eDateMenu.substr(0, 2);
    let eDateMonth = eDateMenu.substr(3, 2);
    let eDateYear = eDateMenu.substr(6, 4);
    this.editedMenu.date = eDateYear + "-" + eDateMonth + "-" + eDateDay;
  }

  ngOnInit(): void {
    this.loadMenus();
    this.getNameProvider();
    this.isAdminMyRole = this.rolesServ.isAdminRole();
  }

  getNameProvider() {
    this.providerServ.getPrivoder(this.providerId).subscribe((data: Provider) => {
      this.nameProvider = data.name;
    });
  }

  // load menus
  loadMenus() {
    this.menuServ.getMenuByProviderId(this.providerId).subscribe((data: Menu[]) => {
      this.menus = data;
    });
  }

  // delete menu
  deleteMenu(id: number) {
    this.menuServ.deleteMenu(id).subscribe(response => {
      this.loadMenus();
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
  }

  addMenu() {
    this.editedMenu = new Menu();
    this.menus.push(this.editedMenu);
    this.isView = false;
    this.isNewRecord = true;
  }

  // edit menu
  editMenu(menu: Menu) {
    this.isEdit = true;
    this.isView = false;
    this.editedMenu = menu;
    this.getEditDate();
  }

  refresh() {
    this.searchSelectionString = "Search by";
    this.searchStr = "";
  }

  getMenus(): Array<Menu> {
    if (this.searchSelectionString == "Id") {
      return this.menus.filter(x => x.id.toString().toLowerCase().includes(this.searchStr.toLowerCase()));
    }
    else if (this.searchSelectionString == "Date") {
      return this.menus.filter(x => x.date.toString().toLowerCase().includes(this.searchStr.toLowerCase()));
    }
    else if (this.searchSelectionString == "Information") {
      return this.menus.filter(x => x.info.toLowerCase().includes(this.searchStr.toLowerCase()));
    }

    return this.menus;
  }

  saveMenu() {
    this.editedMenu.providerId = Number(this.providerId);
    // add menu
    if (this.isNewRecord) {
      this.menuServ.createMenu(this.editedMenu).subscribe(response => {
        this.loadMenus();
        this.statusMessage = 'Data added successfully';
        this.isMessInfo = true;
        this.isShowStatusMessage = true;
      } , err => {
        this.menus.pop();
          this.statusMessage = 'Error adding data';
          if (typeof err.error == 'string') {
            this.statusMessage += '. ' + err.error;
          }
          this.isMessInfo = false;
          this.isShowStatusMessage = true;
          console.log(err);
        });
    } else {
      // edit menu
      this.menuServ.updateMenu(this.editedMenu).subscribe(response => {
        this.loadMenus();
        this.statusMessage = 'Data changed successfully';
        this.isMessInfo = true;
        this.isShowStatusMessage = true;
      }, err => {
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

  cancel() {
    if (this.isEdit) {
      this.editedMenu.date = this.editOldDate;
    }

    // if cancel for add, delete last menu
    if (this.isNewRecord) {
      this.menus.pop();
    }
    this.isNewRecord = false;
    this.isView = true;
    this.editedMenu = null;
    this.isEdit = false;
  }

}
