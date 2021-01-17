import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { Menu } from '../menu/menu';


@Injectable()
export class MenuService {

    private url = "https://localhost:44342/api/menu";

    constructor(private http: HttpClient) { }

    getMenuByProviderId(providerid: number) {
        return this.http.get(this.url + '/provider/' + providerid);
    }

    deleteMenu(id: number) {
        const myHeaders = new HttpHeaders().set("Content-Type", "application/json");
        return this.http.delete(this.url + '/' + id, { headers: myHeaders, observe: 'response' });
    }

    createMenu(menu: Menu) {
        const myHeaders = new HttpHeaders().set("Content-Type", "application/json");
        return this.http.post(this.url, JSON.stringify(menu), { headers: myHeaders, observe: 'response' });
    }

    updateMenu(menu: Menu) {
        const myHeaders = new HttpHeaders().set("Content-Type", "application/json");
        return this.http.put(this.url, JSON.stringify(menu), { headers: myHeaders, observe: 'response' });
    }

    getMenu(id: number) {
        return this.http.get(this.url + "/" + id);
    }
}
