import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { Catalog } from '../catalog/catalog';


@Injectable()
export class CatalogService {

    private url = "https://localhost:44342/api/catalog";

    constructor(private http: HttpClient) { }

    getCatalog(id: number) {
        return this.http.get(this.url + "/" + id);
    }

    getCatalogsByProviderId(providerid: number) {
        return this.http.get(this.url + '/provider/' + providerid);
    }

    deleteCatalog(id: number) {
        const myHeaders = new HttpHeaders().set("Content-Type", "application/json");
        return this.http.delete(this.url + '/' + id, { headers: myHeaders, observe: 'response' });
    }

    createCatalog(catalog: Catalog) {
        const myHeaders = new HttpHeaders().set("Content-Type", "application/json");
        return this.http.post(this.url, JSON.stringify(catalog), { headers: myHeaders, observe: 'response' });
    }

    updateCatalog(catalog: Catalog) {
        const myHeaders = new HttpHeaders().set("Content-Type", "application/json");
        return this.http.put(this.url, JSON.stringify(catalog), { headers: myHeaders, observe: 'response' });
    }
}
