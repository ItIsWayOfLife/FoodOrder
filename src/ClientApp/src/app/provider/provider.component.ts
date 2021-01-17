import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Title } from '@angular/platform-browser';

import { ProviderService } from '../service/provider.service';
import { RolesService } from '../service/roles.service';

import { Provider } from './provider';

@Component({
  selector: 'app-provider',
  templateUrl: './provider.component.html',
  styleUrls: ['./provider.component.css'],
  providers: [ProviderService, RolesService]
})
export class ProviderComponent implements OnInit {

  providers: Array<Provider>;

  isEdit: boolean;
  isView: boolean;

  editedProvider: Provider;
  isNewRecord: boolean;

  isAdminMyRole: boolean;

  searchSelectionString: string;
  searchStr: string;

  isShowStatusMessage: boolean;
  statusMessage: string;
  isMessInfo: boolean;

  fileName: string;

  constructor(private router: Router,
    private activateRoute: ActivatedRoute,
    private providerServ: ProviderService,
    private rolesServ: RolesService,
    private titleService: Title) {

    this.titleService.setTitle('Provider');

    this.fileName = "";

    this.providers = new Array<Provider>();

    this.isNewRecord = false;
    this.editedProvider = new Provider();

    this.isView = true;
    this.isEdit = false;

    this.searchSelectionString = "Search by";
    this.searchStr = "";

    this.isShowStatusMessage = false;
  }

  ngOnInit(): void {
    this.loadProviders();
    this.isAdminMyRole = this.rolesServ.isAdminOrEmployeeRole();
  }

  // load providers
  loadProviders() {
    this.providerServ.getProviders().subscribe((data: Provider[]) => {
      this.providers = data;
    });
  }

  getProviders(): Array<Provider> {
    if (this.searchSelectionString == "Id") {
      return this.providers.filter(x => x.id.toString().toLowerCase().includes(this.searchStr.toLowerCase()));
    }
    else if (this.searchSelectionString == "Name") {
      return this.providers.filter(x => x.name.toLowerCase().includes(this.searchStr.toLowerCase()));
    }
    else if (this.searchSelectionString == "Email") {
      return this.providers.filter(x => x.email.toLowerCase().includes(this.searchStr.toLowerCase()));
    }
    else if (this.searchSelectionString == "Working days") {
      return this.providers.filter(x => x.workingDays.toLowerCase().includes(this.searchStr.toLowerCase()));
    }
    else if (this.searchSelectionString == "Working hours since") {
      return this.providers.filter(x => x.timeWorkWith.toLowerCase().includes(this.searchStr.toLowerCase()));
    }
    else if (this.searchSelectionString == "Opening hours until") {
      return this.providers.filter(x => x.timeWorkTo.toLowerCase().includes(this.searchStr.toLowerCase()));
    }
    else if (this.searchSelectionString == "Status") {
      let activeStr = "Work";
      let notActiveStr = "No work";
      if (activeStr.toLowerCase().startsWith(this.searchStr.toLowerCase()) && (this.searchStr != "")) {
        return this.providers.filter(x => x.isActive);
      }
      else if (notActiveStr.toLowerCase().startsWith(this.searchStr.toLowerCase()) && (this.searchStr != "")) {
        return this.providers.filter(x => x.isActive == false);
      }
    }
    else if (this.searchSelectionString == "Information") {
      return this.providers.filter(x => x.info.toLowerCase().includes(this.searchStr.toLowerCase()));
    }

    return this.providers;
  }

  refresh() {
    this.searchSelectionString = "Search by";
    this.searchStr = "";
  }

  addProvider() {
    this.editedProvider = new Provider();
    this.providers.push(this.editedProvider);
    this.isView = false;
    this.isNewRecord = true;
  }

  deleteProvider(id: number) {
    this.providerServ.deleteProvider(id).subscribe(response => {
      this.statusMessage = 'Data deleted successfully';
      this.loadProviders();
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

  // edit provider
  editProvider(provider: Provider) {
    this.isEdit = true;
    this.isView = false;
    this.editedProvider = provider;

  }

  saveProvider() {
    // add provider
    if (this.isNewRecord) {
      this.editedProvider.path = this.fileName;
      this.providerServ.createProvider(this.editedProvider).subscribe(response => {
        this.loadProviders();
        this.statusMessage = 'Data added successfully';
        this.isMessInfo = true;
        this.isShowStatusMessage = true;
      }, err => {
          this.providers.pop();
          this.statusMessage = 'Error adding data';
          if (typeof err.error == 'string') {
            this.statusMessage += '. ' + err.error;
          }
          this.isMessInfo = false;
          this.isShowStatusMessage = true;
          console.log(err);
        });
    }
    else {
      if (this.fileName != "") {
        this.editedProvider.path = this.fileName;
      }
      // edit provider
      this.providerServ.updateProvider(this.editedProvider).subscribe(response => {

        this.loadProviders();
        this.statusMessage = 'Data changed successfully';
        this.isMessInfo = true;
        this.isShowStatusMessage = true;
        console.log(response.status);

        this.isMessInfo = true;
      }, err => {
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
    // if cancel for add, delete last provider
    if (this.isNewRecord) {
      this.providers.pop();
    }
    this.isNewRecord = false;
    this.isView = true;
    this.editedProvider = null;
    this.fileName = "";
    this.isEdit = false;
  }

  onNotify(message: string): void {
    this.fileName = message;
  }

}
