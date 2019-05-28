import { Injectable } from '@angular/core';
import { HttpClient, HttpBackend } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map, subscribeOn } from 'rxjs/operators';
import { User } from '../_models/user';
import { AdminService } from './admin.service';
import {JwtHelperService} from '@auth0/angular-jwt';
import { isSameMonth } from 'ngx-bootstrap';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  roles: any [];
  baseUrl = 'https://wpms.azurewebsites.net/api/';
  jwtHelper = new JwtHelperService();
  decodedToken: any;

constructor(private http: HttpClient) { }

login(model: any) {
  return this.http.post(this.baseUrl + 'login', model)
  .pipe(map((respone: any) => {
    const user = respone;
    this.decodedToken = this.jwtHelper.decodeToken(user.token);
    if (user) {
      localStorage.setItem('token', user.token);
  }
   })
  );
  }

  FirstLogin(model: any) {
    return this.http.post(this.baseUrl + "firstlogin", model)
      .pipe(map((response: any) => {
        const user = response;
        this.decodedToken = this.jwtHelper.decodeToken(user.token);
        if (user) {
          localStorage.setItem('token', user.token);
        }
      })
      );
  }

  secondLogin(model: any, id: number) {
    return this.http.post(this.baseUrl + "loginorganization/" + id, model)
    .pipe(map((response: any) => {
      const user = response;
      this.decodedToken = this.jwtHelper.decodeToken(user.token);
      if (user) {
        localStorage.setItem('token', user.token);
      }
    })
    );
  }

  loggedIn() {
    const token = localStorage.getItem('token');
    return !this.jwtHelper.isTokenExpired(token);
  }

  roleMatch(allowedRoles): boolean {
    let isMatch = false;
    const userRoles = this.decodedToken.role as Array<string>;
    allowedRoles.foreach(element => {
      if (userRoles.includes(element)) {
      isMatch = true;
      return;
      }
    });
    return isMatch;
  }

  changeOrg(orgId: any) {
    return this.http.get(this.baseUrl + "changeorganizationtoken/" + orgId)
    .pipe(map((response: any) => {
      const user = response;
      this.decodedToken = this.jwtHelper.decodeToken(user.token);
      if (user) {
        localStorage.setItem('token', user.token);
      }
    })
    );
  }

  isSystemAdmin() {
    return this.http.get(this.baseUrl + "issystemadmin");
  }

  getLoggedInUser(){
    return this.http.get(this.baseUrl + "loggedinuser");
  }

  getLoggedInUsersRole() {
    return this.http.get(this.baseUrl + "loggedinuserroles");
  }

}
