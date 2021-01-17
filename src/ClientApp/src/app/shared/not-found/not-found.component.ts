import { Component, OnInit, OnDestroy  } from '@angular/core';
import { Router } from '@angular/router';
import { Title } from '@angular/platform-browser';

@Component({
  selector: 'app-not-found',
  templateUrl: './not-found.component.html',
  styleUrls: ['./not-found.component.css']
})
export class NotFoundComponent implements OnInit, OnDestroy  {

  constructor(private router: Router, private titleService: Title) { }

  private timer: any;

  ngOnInit() {
    this.titleService.setTitle('Page not found');
    const delay = 5000;
    this.timer = setTimeout(() => {
      this.router.navigate(['']);
     }, delay);
  }

  ngOnDestroy(){
    clearTimeout(this.timer);
  }

}
