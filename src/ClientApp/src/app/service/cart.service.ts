import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { CartDishesUpdate } from '../cart/cartDishesUpdate';


@Injectable()
export class CartService {

    private url = "https://localhost:44342/api/cart";

    constructor(private http: HttpClient) { }

    getDishInCart() {
        return this.http.get(this.url + '/dishes');
    }

    updateCart(cartUpdate: Array<CartDishesUpdate>) {
        const myHeaders = new HttpHeaders().set("Content-Type", "application/json");
        return this.http.put(this.url, JSON.stringify(cartUpdate), { responseType: 'text',
         headers: myHeaders, observe: 'response' });
    }

    deleteDishInCart(id: number) {
        const myHeaders = new HttpHeaders().set("Content-Type", "application/json");
        return this.http.delete(this.url + '/' + id, { headers: myHeaders, observe: 'response' });
    }

    addDishToCart(id: number) {
        const myHeaders = new HttpHeaders().set("Content-Type", "application/json");
        return this.http.post(this.url + '/' + id, { headers: myHeaders, observe: 'response' });
    }

    deleteAllDishesInCart() {
        const myHeaders = new HttpHeaders().set("Content-Type", "application/json");
        return this.http.delete(this.url + '/all/delete', { headers: myHeaders, observe: 'response' });
    }
}
