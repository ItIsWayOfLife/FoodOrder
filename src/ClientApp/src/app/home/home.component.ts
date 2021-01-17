import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';

import { ProviderService } from '../service/provider.service';

import { Provider } from '../provider/provider';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
  providers: [ProviderService]
})
export class HomeComponent implements OnInit {

  providers: Array<Provider>;

  constructor(private providerServ: ProviderService,
    private titleService: Title) {

    this.titleService.setTitle('Home');
    this.providers = new Array<Provider>();
  }

  ngOnInit(): void {
    this.loadFavProviders();
  }

  // load providers
  loadFavProviders() {
    this.providerServ.getProviders().subscribe((data: Provider[]) => {
      this.providers = data.filter(p => p.isFavorite);
    });
  }

}
