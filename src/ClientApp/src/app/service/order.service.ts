import {Injectable} from '@angular/core';
import {HttpClient, HttpHeaders} from '@angular/common/http';

@Injectable()
export class OrderService{

    private url = "https://localhost:44342/api/order";

    constructor(private http: HttpClient) {}

    getOrder(){
        return this.http.get(this.url);
    }

    getOrderDishById(id:number){
        return this.http.get(this.url+'/'+id);
    }

    createOrder(){
        const myHeaders = new HttpHeaders().set("Content-Type", "application/json");
        return this.http.post(this.url, {headers: myHeaders, observe: 'response'}); 
    }
}