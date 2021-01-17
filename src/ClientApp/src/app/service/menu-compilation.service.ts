import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { MakeMenu } from '../menu/makeMenu';
@Injectable()
export class MenuCompilationService {

    private url = "https://localhost:44342/api/menu";

    menuId: number;

    constructor(private http: HttpClient) {
        this.menuId = 0;
    }

    appointMenuId(id: number) {
        this.menuId = id;
    }

    isComplition(): boolean {
        if (this.menuId <= 0) {
            return false;
        }
        else {
            return true;
        }
    }

    getDishesInMenu() {
        return this.http.get(this.url + '/dishes/' + this.menuId);
    }

    createMenu(makeMenuModel: MakeMenu) {
        const myHeaders = new HttpHeaders().set("Content-Type", "application/json");
        let newMakeMenu: MakeMenu = new MakeMenu();
        newMakeMenu.menuId = Number(this.menuId);
        newMakeMenu.allSelect = makeMenuModel.allSelect;
        newMakeMenu.newAddedDishes = makeMenuModel.newAddedDishes;
        return this.http.post(this.url + "/makemenu", JSON.stringify(newMakeMenu), { headers: myHeaders, observe: 'response' });
    }
}
