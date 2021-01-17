import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { Provider } from '../provider/provider';

@Injectable()
export class ProviderService {

    private url = "https://localhost:44342/api/provider";

    constructor(private http: HttpClient) { }

    getPrivoder(id: number) {
        return this.http.get(this.url + "/" + id);
    }

    getProviders() {
        return this.http.get(this.url);
    }

    createProvider(provider: Provider) {
        const myHeaders = new HttpHeaders().set("Content-Type", "application/json");
        return this.http.post(this.url, JSON.stringify(provider), { headers: myHeaders, observe: 'response' });
    }

    deleteProvider(id: number) {
        const myHeaders = new HttpHeaders().set("Content-Type", "application/json");
        return this.http.delete(this.url + '/' + id, { headers: myHeaders, observe: 'response' });
    }

    updateProvider(provider: Provider) {
        const myHeaders = new HttpHeaders().set("Content-Type", "application/json");
        return this.http.put(this.url, JSON.stringify(provider), { headers: myHeaders, observe: 'response' });
    }
}
