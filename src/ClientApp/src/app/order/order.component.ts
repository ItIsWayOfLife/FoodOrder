import { Component, OnInit, DoCheck } from '@angular/core';
import { Location } from '@angular/common';
import { Title } from '@angular/platform-browser';

import { OrderService } from '../service/order.service';

import { Order } from './order';
import { OrderDishes } from './OrderDishes';

@Component({
  selector: 'app-order',
  templateUrl: './order.component.html',
  styleUrls: ['./order.component.css'],
  providers: [OrderService]
})
export class OrderComponent implements OnInit {

  orders: Array<Order>;
  orderDishes: Array<OrderDishes>;

  isViewOrdersList: boolean;

  idOrderViewDishes: number;

  searchSelectionString: string;
  searchStr: string;

  allPrice: number;

  constructor(private orderServ: OrderService,
    private _location: Location,
    private titleService: Title) {

    this.titleService.setTitle('Orders');

    this.orders = new Array<Order>();

    this.isViewOrdersList = true;

    this.orderDishes = new Array<OrderDishes>();

    this.searchSelectionString = "Search by";
    this.searchStr = "";
  }

  ngOnInit(): void {
    this.loadOrders();
  }

  backClicked() {
    this._location.back();
  }

  // load menus
  loadOrders() {
    this.orderServ.getOrder().subscribe((data: Order[]) => {
      this.orders = data;
    });
  }

  getInfoDish(id: number, fullPrice: number) {
    this.allPrice = fullPrice;
    this.orderDishes = new Array<OrderDishes>();
    this.orderServ.getOrderDishById(id).subscribe((data: OrderDishes[]) => {
      this.orderDishes = data;
      this.isViewOrdersList = false;
    });
  }

  getTotalPrice(orderD: OrderDishes) {
    return (orderD.count * orderD.price).toFixed(2);
  }

  back() {
    this.isViewOrdersList = true;
  }

  getOrders(): Array<Order> {
    if (this.searchSelectionString == "Order number") {
      return this.orders.filter(x => x.id.toString().toLowerCase().includes(this.searchStr.toLowerCase()));
    }
    else if (this.searchSelectionString == "Date and time") {
      return this.orders.filter(x => x.dateOrder.toString().toLowerCase().includes(this.searchStr.toLowerCase()));
    }
    else if (this.searchSelectionString == "Number of dishes") {
      return this.orders.filter(x => x.countDish.toString().toLowerCase().includes(this.searchStr.toLowerCase()));
    }
    else if (this.searchSelectionString == "Full price") {
      return this.orders.filter(x => x.fullPrice.toString().toLowerCase().includes(this.searchStr.toLowerCase()));
    }

    return this.orders;
  }

  refresh() {
    this.searchSelectionString = "Search by";
    this.searchStr = "";
  }

}
