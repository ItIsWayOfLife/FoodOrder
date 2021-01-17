import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';

@Component({
  selector: 'app-about-delivery',
  templateUrl: './about-delivery.component.html',
  styleUrls: ['./about-delivery.component.scss']
})
export class AboutDeliveryComponent implements OnInit {

  constructor(private titleService: Title) {

    this.titleService.setTitle('About delivery');
  }
  ngOnInit() {
  }

}
