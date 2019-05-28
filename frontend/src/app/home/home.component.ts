import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})

export class HomeComponent implements OnInit {
  loggedInUser: any;
  loggedInRole: any;
  isCrm: boolean = false;
  isPms: boolean = false;

  constructor(public _authService: AuthService) { }

  ngOnInit() {
    this.getCurrentUser();
    this.getLoggedInUsersRoles();
  }

  getCurrentUser() {
    this._authService.getLoggedInUser()
      .subscribe(data => this.loggedInUser = data);
  }

  getLoggedInUsersRoles() {
    this._authService.getLoggedInUsersRole()
      .subscribe(data => {
        this.loggedInRole = data;
        if (this.loggedInRole.name == "CRM") {
          this.isCrm = true;
          this.isPms = false;
        }
        else if (this.loggedInRole.name == "PMS") {
          this.isPms = true;
          this.isCrm = false;
        }
      });
  }

}
