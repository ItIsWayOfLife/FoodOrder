import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';
import { Title } from '@angular/platform-browser';

import { DishService } from '../service/dish.service';
import { RolesService } from '../service/roles.service';
import { MenuService } from '../service/menu.service';
import { CartService } from '../service/cart.service';
import { MenuCompilationService } from '../service/menu-compilation.service';

import { MenuDishes } from './menuDishes';
import { Menu } from '../menu/menu';

@Component({
  selector: 'app-menu-dishes',
  templateUrl: './menu-dishes.component.html',
  styleUrls: ['./menu-dishes.component.css'],
  providers: [RolesService,
    DishService,
    MenuService,
    CartService]
})
export class MenuDishesComponent implements OnInit {

  menuId: number;

  isAdminMyRole: boolean;
  isAdminOrEmployeeMyRole: boolean;

  menuDishes: Array<MenuDishes>;

  dateMenu: string;

  isShowStatusMessage: boolean;
  statusMessage: string;
  isMessInfo: boolean;

  searchSelectionString: string;
  searchStr: string;

  idProvider: number;

  isCompilation: boolean;

  constructor(private router: Router,
    private activateRoute: ActivatedRoute,
    private dishServ: DishService,
    private rolesServ: RolesService,
    private menuServ: MenuService,
    private cartServ: CartService,
    private _location: Location,
    private menuCompilationServ: MenuCompilationService,
    private titleService: Title) {

    this.titleService.setTitle('Menu dishes');

    this.menuId = activateRoute.snapshot.params['menuId'];

    this.menuDishes = new Array<MenuDishes>();

    this.searchSelectionString = "Search by";
    this.searchStr = "";

    this.isShowStatusMessage = false;

    this.isCompilation = this.menuCompilationServ.isComplition();
  }

  ngOnInit(): void {
    this.loadMenuDishes();
    this.getDateMenu();
    this.isAdminMyRole = this.rolesServ.isAdminRole();
    this.isAdminOrEmployeeMyRole = this.rolesServ.isAdminOrEmployeeRole();
    this.getProviderId();
  }

  backClicked() {
    this._location.back();
  }

  getDateMenu() {
    this.menuServ.getMenu(this.menuId).subscribe((data: Menu) => {
      this.dateMenu = data.date;
    });
  }

  // load menuDishes
  loadMenuDishes() {
    this.dishServ.getDishesByMenuId(this.menuId).subscribe((data: MenuDishes[]) => {
      this.menuDishes = data;
    });
  }

  getMenuDishes(): Array<MenuDishes> {
    if (this.searchSelectionString == "Id ") {
      return this.menuDishes.filter(x => x.dishId.toString().toLowerCase().includes(this.searchStr.toLowerCase()));
    }
    else if (this.searchSelectionString == "Id catalog") {
      return this.menuDishes.filter(x => x.catalogId.toString().toLowerCase().includes(this.searchStr.toLowerCase()));
    }
    else if (this.searchSelectionString == "Name") {
      return this.menuDishes.filter(x => x.name.toString().toLowerCase().includes(this.searchStr.toLowerCase()));
    }
    else if (this.searchSelectionString == "Information") {
      return this.menuDishes.filter(x => x.info.toLowerCase().includes(this.searchStr.toLowerCase()));
    }
    else if (this.searchSelectionString == "Weight") {
      return this.menuDishes.filter(x => x.weight.toString().toLowerCase().includes(this.searchStr.toLowerCase()));
    }
    else if (this.searchSelectionString == "Price") {
      return this.menuDishes.filter(x => x.price.toString().toLowerCase().includes(this.searchStr.toLowerCase()));
    }

    return this.menuDishes;
  }

  refresh() {
    this.searchSelectionString = "Search by";
    this.searchStr = "";
  }

  addToCart(idDish: number) {
    this.cartServ.addDishToCart(idDish).subscribe(response => {
      this.router.navigate(["cart"]);
    }, err => {
        this.statusMessage = 'Error adding dish to cart';
        if (typeof err.error == 'string') {
          this.statusMessage += '. ' + err.error;
        }
        this.isMessInfo = false;
        this.isShowStatusMessage = true;
        console.log(err);
      });
  }

  menuCompilationCancel() {
    this.menuCompilationServ.appointMenuId(0);
    this.isCompilation = false;
  }

  getProviderId() {
    this.menuServ.getMenu(this.menuId).subscribe((data: Menu) => {
      this.idProvider = data.providerId;
    });
  }

  menuCompilation() {
    this.getProviderId();
    this.menuCompilationServ.appointMenuId(this.menuId);
    this.isCompilation = this.menuCompilationServ.isComplition();
    this.router.navigate(["/catalog/" + this.idProvider]);
  }

  // show info
  showStatusMess() {
    this.isShowStatusMessage = !this.isShowStatusMessage;
  }
}
