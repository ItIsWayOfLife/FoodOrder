import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';
import { Title } from '@angular/platform-browser';

import { CatalogService } from '../service/catalog.service';
import { RolesService } from '../service/roles.service';
import { ProviderService } from '../service/provider.service';

import { Provider } from '../provider/provider';
import { Catalog } from './catalog';

@Component({
  selector: 'app-catalog',
  templateUrl: './catalog.component.html',
  styleUrls: ['./catalog.component.css'],
  providers: [RolesService,
    CatalogService,
    ProviderService]
})
export class CatalogComponent implements OnInit {

  providerId: number;

  isAdminMyRole: boolean;

  editedCatalog: Catalog;
  catalogs: Array<Catalog>;

  isShowStatusMessage: boolean;
  statusMessage: string;
  isMessInfo: boolean;

  nameProvider: string;

  searchSelectionString: string;
  searchStr: string;

  isEdit: boolean;
  isView: boolean;
  isNewRecord: boolean;

  constructor(private activateRoute: ActivatedRoute,
    private catalogServ: CatalogService,
    private rolesServ: RolesService,
    private providerServ: ProviderService,
    private _location: Location,
    private titleService: Title) {

    this.titleService.setTitle('Catalog');

    this.providerId = activateRoute.snapshot.params['providerId'];

    this.catalogs = new Array<Catalog>();
    this.editedCatalog = new Catalog();

    this.isShowStatusMessage = false;

    this.searchSelectionString = "Search by";
    this.searchStr = "";

    this.isView = true;
    this.isEdit = false;
    this.isNewRecord = false;
  }

  ngOnInit(): void {
    this.loadCatalog();
    this.getNameProvider();
    this.isAdminMyRole = this.rolesServ.isAdminRole();
  }

  backClicked() {
    this._location.back();
  }

  getNameProvider() {
    this.providerServ.getPrivoder(this.providerId).subscribe((data: Provider) => {
      this.nameProvider = data.name;
    });
  }

  // load catalog
  loadCatalog() {
    this.catalogServ.getCatalogsByProviderId(this.providerId).subscribe((data: Catalog[]) => {
      this.catalogs = data;
    });
  }

  // delete catalog
  deleteCatalog(id: number) {
    this.catalogServ.deleteCatalog(id).subscribe(response => {
      this.loadCatalog();
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

  addCatalog() {
    this.editedCatalog = new Catalog();
    this.catalogs.push(this.editedCatalog);
    this.isView = false;
    this.isNewRecord = true;
  }

  // edit catalog
  editCatalog(catalog: Catalog) {
    this.isEdit = true;
    this.isView = false;
    this.editedCatalog = catalog;
  }

  refresh() {
    this.searchSelectionString = "Search by";
    this.searchStr = "";
  }

  getCatalogs(): Array<Catalog> {
    if (this.searchSelectionString == "Id") {
      return this.catalogs.filter(x => x.id.toString().toLowerCase().includes(this.searchStr.toLowerCase()));
    }
    else if (this.searchSelectionString == "Name") {
      return this.catalogs.filter(x => x.name.toLowerCase().includes(this.searchStr.toLowerCase()));
    }
    else if (this.searchSelectionString == "Information") {
      return this.catalogs.filter(x => x.info.toLowerCase().includes(this.searchStr.toLowerCase()));
    }

    return this.catalogs;
  }

  saveCatalog() {
    this.editedCatalog.providerId = Number(this.providerId);
    // add catalog
    if (this.isNewRecord) {
      this.catalogServ.createCatalog(this.editedCatalog).subscribe(response => {
        this.loadCatalog();
        this.statusMessage = 'Data added successfully';
        this.isMessInfo = true;
        this.isShowStatusMessage = true;
      }, err => {
        this.catalogs.pop();
        this.statusMessage = 'Error adding data';
        if (typeof err.error == 'string') {
          this.statusMessage += '. ' + err.error;
        }
        this.isMessInfo = false;
        this.isShowStatusMessage = true;
        console.log(err);
      });
    } else {
      // edit catalog
      this.catalogServ.updateCatalog(this.editedCatalog).subscribe(response => {
        this.loadCatalog();
        this.statusMessage = 'Data changed successfully';
        this.isMessInfo = true;
        this.isShowStatusMessage = true;
      }, err => {
        this.loadCatalog();
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
    // if cancel for add, delete last catalog
    if (this.isNewRecord) {
      this.catalogs.pop();
    }
    this.isNewRecord = false;
    this.isView = true;
    this.editedCatalog = null;
    this.isEdit = false;
  }

}
