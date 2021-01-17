import { Component, OnInit, Inject } from '@angular/core';
import { Title } from '@angular/platform-browser';

import { ReportService } from '../service/report.service';

@Component({
  selector: 'app-report',
  templateUrl: './report.component.html',
  styleUrls: ['./report.component.css'],
  providers: [ReportService]
})
export class ReportComponent implements OnInit {

  providerId: number;
  userId: string;

  isViewProviders: boolean;
  isViewUsers: boolean;

  dateWith: Date;
  dateTo: Date;

  reportFile: File;

  constructor(private reportServ: ReportService,
    private titleService: Title) {

    this.titleService.setTitle('Reports');

    this.isViewProviders = false;
    this.isViewUsers = false;
  }

  ngOnInit(): void {
  }

  dateRefresh() {
    this.dateTo = new Date();
    this.dateWith = new Date();
  }

  viewProviders() {
    this.dateRefresh();
    this.isViewProviders = !this.isViewProviders;
    if (this.isViewProviders) {
      this.isViewUsers = false;
    }
  }

  viewUsers() {
    this.dateRefresh();
    this.isViewUsers = !this.isViewUsers;
    if (this.isViewUsers) {
      this.isViewProviders = false;
    }
  }

  // for providers
  //
  getReportProviderAllTime() {
    this.reportServ.getReportProviderAllTime(this.providerId).subscribe(
      (res) => {
        const blob = new Blob([res], { type: "application/pdf" });
        var fileURL = URL.createObjectURL(blob);
        window.open(fileURL);
      });
  }

  getReportProviderDate() {
    this.reportServ.getReportProviderDate(this.providerId, this.dateWith).subscribe(
      (res) => {
        const blob = new Blob([res], { type: "application/pdf" });
        var fileURL = URL.createObjectURL(blob);
        window.open(fileURL);
      });
  }

  getReportProviderWithToDate() {
    this.reportServ.getReportProviderWithToDate(this.providerId, this.dateWith, this.dateTo).subscribe(
      (res) => {
        const blob = new Blob([res], { type: "application/pdf" });
        var fileURL = URL.createObjectURL(blob);
        window.open(fileURL);
      });
  }

  getReportProvidersAllTime() {
    this.reportServ.getReportProvidersAllTime().subscribe(
      (res) => {
        const blob = new Blob([res], { type: "application/pdf" });
        var fileURL = URL.createObjectURL(blob);
        window.open(fileURL);
      });
  }

  getReportProvidersDate() {
    this.reportServ.getReportProvidersDate(this.dateWith).subscribe(
      (res) => {
        const blob = new Blob([res], { type: "application/pdf" });
        var fileURL = URL.createObjectURL(blob);
        window.open(fileURL);
      });
  }

  getReportProvidersWithToDate() {
    this.reportServ.getReportProvidersWithToDate(this.dateWith, this.dateTo).subscribe(
      (res) => {
        const blob = new Blob([res], { type: "application/pdf" });
        var fileURL = URL.createObjectURL(blob);
        window.open(fileURL);
      });
  }
  //

  // for users
  //
  getReportUserAllTime() {
    this.reportServ.getReportUserAllTime(this.userId).subscribe(
      (res) => {
        const blob = new Blob([res], { type: "application/pdf" });
        var fileURL = URL.createObjectURL(blob);
        window.open(fileURL);
      });
  }

  getReportUserDate() {
    this.reportServ.getReportUserDate(this.userId, this.dateWith).subscribe(
      (res) => {
        const blob = new Blob([res], { type: "application/pdf" });
        var fileURL = URL.createObjectURL(blob);
        window.open(fileURL);
      });
  }

  getReportUserWithToDate() {
    this.reportServ.getReportUserWithToDate(this.userId, this.dateWith, this.dateTo).subscribe(
      (res) => {
        const blob = new Blob([res], { type: "application/pdf" });
        var fileURL = URL.createObjectURL(blob);
        window.open(fileURL);
      });
  }

  getReportUsersAllTime() {
    this.reportServ.getReportUsersAllTime().subscribe(
      (res) => {
        const blob = new Blob([res], { type: "application/pdf" });
        var fileURL = URL.createObjectURL(blob);
        window.open(fileURL);
      });
  }

  getReportUsersDate() {
    this.reportServ.getReportUsersDate(this.dateWith).subscribe(
      (res) => {
        const blob = new Blob([res], { type: "application/pdf" });
        var fileURL = URL.createObjectURL(blob);
        window.open(fileURL);
      });
  }

  getReportUsersWithToDate() {
    this.reportServ.getReportUsersWithToDate(this.dateWith, this.dateTo).subscribe(
      (res) => {
        const blob = new Blob([res], { type: "application/pdf" });
        var fileURL = URL.createObjectURL(blob);
        window.open(fileURL);
      });
  }
  //
}
