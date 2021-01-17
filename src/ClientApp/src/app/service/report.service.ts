import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class ReportService {

    private url = "https://localhost:44342/api/report";

    constructor(private http: HttpClient) { }

    // for providers
    //
    getReportProviderAllTime(providerid: number) {
        return this.http.get(this.url + "/provider/" + providerid, { responseType: 'arraybuffer' });
    }

    getReportProviderDate(providerid: number, date: Date) {
        return this.http.get(this.url + "/provider/" + providerid + "/" + date, { responseType: 'arraybuffer' });
    }

    getReportProviderWithToDate(providerid: number, dateWith: Date, dateTo: Date) {
        return this.http.get(this.url + "/provider/" + providerid + "/" + dateWith + "/" + dateTo, { responseType: 'arraybuffer' });
    }


    getReportProvidersAllTime() {
        return this.http.get(this.url + "/providers", { responseType: 'arraybuffer' });
    }

    getReportProvidersDate(date: Date) {
        return this.http.get(this.url + "/providers/" + date, { responseType: 'arraybuffer' });
    }

    getReportProvidersWithToDate(dateWith: Date, dateTo: Date) {
        return this.http.get(this.url + "/providers/" + dateWith + "/" + dateTo, { responseType: 'arraybuffer' });
    }
    //

    // for users
    //
    getReportUserAllTime(userid: string) {
        return this.http.get(this.url + "/user/" + userid, { responseType: 'arraybuffer' });
    }

    getReportUserDate(userid: string, date: Date) {
        return this.http.get(this.url + "/user/" + userid + "/" + date, { responseType: 'arraybuffer' });
    }

    getReportUserWithToDate(userid: string, dateWith: Date, dateTo: Date) {
        return this.http.get(this.url + "/user/" + userid + "/" + dateWith + "/" + dateTo, { responseType: 'arraybuffer' });
    }


    getReportUsersAllTime() {
        return this.http.get(this.url + "/users", { responseType: 'arraybuffer' });
    }

    getReportUsersDate(date: Date) {
        return this.http.get(this.url + "/users/" + date, { responseType: 'arraybuffer' });
    }

    getReportUsersWithToDate(dateWith: Date, dateTo: Date) {
        return this.http.get(this.url + "/users/" + dateWith + "/" + dateTo, { responseType: 'arraybuffer' });
    }
    //
}
