import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';
import { Title } from '@angular/platform-browser';

import { DishService } from '../service/dish.service';
import { RolesService } from '../service/roles.service';
import { CatalogService } from '../service/catalog.service';

import { MenuCompilationService } from '../service/menu-compilation.service';

import { Dish } from './dish';
import { Catalog } from '../catalog/catalog';
import { MakeMenu } from '../menu/makeMenu';

@Component({
  selector: 'app-dish',
  templateUrl: './dish.component.html',
  styleUrls: ['./dish.component.css'],
  providers: [RolesService,
    DishService,
    CatalogService]
})
export class DishComponent implements OnInit {

  catalogId: number;

  isAdminMyRole: boolean;

  editedDish: Dish;
  dishes: Array<Dish>;

  isShowStatusMessage: boolean;
  statusMessage: string;
  isMessInfo: boolean;

  nameCatalog: string;

  searchSelectionString: string;
  searchStr: string;

  isEdit: boolean;
  isView: boolean;
  isNewRecord: boolean;

  isCompilation: boolean;

  fileName: string;

  listDishesInMenu: Array<number>;

  constructor(private activateRoute: ActivatedRoute,
    private dishServ: DishService,
    private rolesServ: RolesService,
    private catalogServ: CatalogService,
    private _location: Location,
    private menuCompilationServ: MenuCompilationService,
    private titleService: Title) {

    this.titleService.setTitle('Dishes');

    this.fileName = "";
    this.catalogId = activateRoute.snapshot.params['catalogId'];

    this.dishes = new Array<Dish>();
    this.editedDish = new Dish();

    this.isShowStatusMessage = false;

    this.searchSelectionString = "Search by";
    this.searchStr = "";

    this.isView = true;
    this.isEdit = false;
    this.isNewRecord = false;
    this.isCompilation = false;

    this.isCompilation = this.menuCompilationServ.isComplition();

    this.listDishesInMenu = new Array<number>();
  }

  backClicked() {
    this._location.back();
  }

  ngOnInit(): void {
    this.loadDishes();
    this.getNameCatalog();
    this.isAdminMyRole = this.rolesServ.isAdminRole();
  }

  loadtDishesInMenu() {
    this.menuCompilationServ.getDishesInMenu().subscribe((data: number[]) => {
      this.listDishesInMenu = data;

      // create models for add menu
      for (let d of this.dishes) {
        let bl: boolean;

        if (data.includes(d.id)) {
          bl = true;
        }
        else {
          bl = false;
        }

        if (bl) {
          d.addMenu = true;
        }
        else {
          d.addMenu = false;
        }
      }
    });
  }

  getNameCatalog() {
    this.catalogServ.getCatalog(this.catalogId).subscribe((data: Catalog) => {
      this.nameCatalog = data.name;
    });
  }

  // load dishes
  loadDishes() {
    this.dishServ.getDishByCatalogId(this.catalogId).subscribe((data: Dish[]) => {
      this.dishes = data;

      if (this.isCompilation ?? this.isAdminMyRole) {
        this.loadtDishesInMenu();
      }
    });
  }

  // delete dish
  deleteDish(id: number) {
    this.dishServ.deleteDish(id).subscribe(response => {
      this.loadDishes();
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

  addDish() {
    this.editedDish = new Dish();
    this.dishes.push(this.editedDish);
    this.isView = false;
    this.isNewRecord = true;
  }

  // edit menu
  editDish(dish: Dish) {
    this.isEdit = true;
    this.isView = false;
    this.editedDish = dish;
  }

  refresh() {
    this.searchSelectionString = "Search by";
    this.searchStr = "";
  }

  getDishes(): Array<Dish> {
    if (this.searchSelectionString == "Id") {
      return this.dishes.filter(x => x.id.toString().toLowerCase().includes(this.searchStr.toLowerCase()));
    }
    else if (this.searchSelectionString == "Name") {
      return this.dishes.filter(x => x.name.toString().toLowerCase().includes(this.searchStr.toLowerCase()));
    }
    else if (this.searchSelectionString == "Information") {
      return this.dishes.filter(x => x.info.toLowerCase().includes(this.searchStr.toLowerCase()));
    }
    else if (this.searchSelectionString == "Weight") {
      return this.dishes.filter(x => x.weight.toString().toLowerCase().includes(this.searchStr.toLowerCase()));
    }
    else if (this.searchSelectionString == "Price") {
      return this.dishes.filter(x => x.price.toString().toLowerCase().includes(this.searchStr.toLowerCase()));
    }

    return this.dishes;
  }

  saveDish() {

    this.editedDish.catalogId = Number(this.catalogId);

    if (this.isNewRecord) {
      // add dish
      this.editedDish.path = this.fileName;
      this.dishServ.createMenu(this.editedDish).subscribe(response => {
        this.loadDishes();
        this.statusMessage = 'Data added successfully';
        this.isMessInfo = true;
        this.isShowStatusMessage = true;
      }, err => {
        this.dishes.pop();
        this.statusMessage = 'Error adding data';
        if (typeof err.error == 'string') {
          this.statusMessage += '. ' + err.error;
        }
        this.isMessInfo = false;
        this.isShowStatusMessage = true;
        console.log(err);
      });
    } else {
      if (this.fileName != "") {
        this.editedDish.path = this.fileName;
      }
      // edit dish
      this.dishServ.updateMenu(this.editedDish).subscribe(response => {
        this.loadDishes();
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
    // if canceled when adding, delete the last entry
    if (this.isNewRecord) {
      this.dishes.pop();
    }
    this.isNewRecord = false;
    this.isView = true;
    this.editedDish = null;
    this.isEdit = false;
  }

  onNotify(message: string): void {
    this.fileName = message;
  }

  addDishToMenu() {
    let allSelect: Array<number> = new Array<number>();
    let newAddSelect: Array<number> = new Array<number>();

    let makeMenu: MakeMenu = new MakeMenu();

    for (let d of this.dishes) {
      allSelect.push(d.id);
      if (d.addMenu) {
        newAddSelect.push(d.id);
      }
    }

    makeMenu.allSelect = allSelect;
    makeMenu.newAddedDishes = newAddSelect;

    this.menuCompilationServ.createMenu(makeMenu).subscribe(response => {
      this.statusMessage = "Menu has been successfully compiled";
      this.isMessInfo = true;
      this.isShowStatusMessage = true;
    }, err => {
      this.statusMessage = "Menu compilation error";
      if (typeof err.error == 'string') {
        this.statusMessage += '. ' + err.error;
      }
      this.isMessInfo = false;
      this.isShowStatusMessage = true;
      console.log(err);
    });
  }

}


