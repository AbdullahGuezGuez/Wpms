import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { User } from '../_models/user';
import { map } from 'rxjs/operators';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  
  private _baseUrl = 'http://localhost:5000/api/';
  roles: any;

  constructor(private http: HttpClient, private authService: AuthService) { }

  register(user: any) {
    return this.http.post('http://localhost:5000/api/admin/register', user).pipe(
      map((response: any) => {
        const name = response;
      })
    );
  }

  resetPassword(value: any) {
    return this.http.put(this._baseUrl + 'admin/resetpassword', value);
  }

  changePassword(values: any) {
    return this.http.put(this._baseUrl + 'admin/changepassword', values);
  }

  maskUser(id: any) {
    return this.http.put(this._baseUrl + 'admin/maskuser',  id);
  }

  getUsersOnRole(id: any) {
    return this.http.get(this._baseUrl + 'role/roleusers/' + id);
  }

  getAllClaims() {
    return this.http.get(this._baseUrl + 'role/claims');
  }

  getAllRoleWithClaims() {
    return this.http.get(this._baseUrl + 'role/roleclaims');
  }

  changeRoleClaims(role: any) {
    return this.http.put(this._baseUrl + 'role/roleclaims', role).subscribe();
  }

  getAllRoles() {
    return this.http.get(this._baseUrl + 'role/role');
  }

  getUsersWithRoles() {
    return this.http.get('http://localhost:5000/api/role/usersWithRoles');
  }

  updateUserRoles(user: any, roles: {}) {
    return this.http.post('http://localhost:5000/api/role/editRoles/' + user, roles);
  }

  // updateUserOrganizations(userId: number) {
  //   this.http.put(this._baseUrl + "hej");
  // }
  
  getRoles() {
    return this.http.get('http://localhost:5000/api/role/getRoles');
  }

  createCustomRole(role: any) {
    return this.http.post(this._baseUrl + 'role/role', role);
  }

  deleteCustomRole(role: any) {
    return this.http.delete(this._baseUrl + 'role/role/' + role).subscribe();
  }

  updateRoleClaims(role: any) {
    return this.http.put(this._baseUrl + 'role/roleclaims', role).subscribe();
  }

  createRole(roleName: any) {
    const url = this._baseUrl + 'role/role';
    return this.http.post(url, roleName);
  }

  deleteRole(id: any) {
    var urlDelete = this._baseUrl + 'role/role/' + id;
    return this.http.delete(urlDelete).subscribe();
  }

  getUsersOrganizations(name: any) {
    return this.http.get(this._baseUrl + "admin/organizationbyuser/" + name).pipe();
  }

  getUsersOrganizationsToken() {
    return this.http.get(this._baseUrl + "admin/organizationbyusertoken/");
  }

  getAllUsers() {
    return this.http.get('http://localhost:5000/api/users/allusersinorganization');
  }

  getUsersOrganizationById(userId: number) {
    return this.http.get(this._baseUrl + "organizationbyuserid/" + userId);
  }

  getAllOrganizations() {
    return this.http.get(this._baseUrl + 'admin/organization').pipe();
  }


  postNewOrganization(model: any) {
    return this.http.post(this._baseUrl + 'admin/organization', model);
  }

  updateOrganization(id: any, body: any) {
    var urlToUpdate = this._baseUrl + 'admin/organization/' + id;
    return this.http.put(urlToUpdate, body).subscribe();
  }

  deleteOrganization(id: any) {
    var urlDelete = this._baseUrl + 'admin/organization/' + id;
    return this.http.delete(urlDelete).subscribe();
  }

  getAllProjects() {
    return this.http.get(this._baseUrl + 'project/projects');
  }

  getProjectList() {
    return this.http.get(this._baseUrl + "project/projectslist/true");
  }

  getUnactiveProjectList() {
    return this.http.get(this._baseUrl + "project/projectslist/false");
  }

  getOneProject(id: string) {
    return this.http.get(this._baseUrl + 'project/project/' + id);
  }
}
