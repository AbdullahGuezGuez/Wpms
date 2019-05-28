import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpBackend } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class CustomerService {
  private _baseUrl = 'http://localhost:5000/api/customer/';
  private _userUrl = 'http://localhost:5000/api/users/';

constructor(private http: HttpClient) { }

 // Create Activity
  getAllUsersInOrganization() {
    return this.http.get(this._userUrl + "allusersinorganization");
  }

  getAllCustomersForOrganization() {
    return this.http.get(this._baseUrl + "customers/true");
  }

  getAllInactiveCustomersForOrganization() {
    return this.http.get(this._baseUrl + "customers/false");
  }

  getCustomerById(id: number) {
    return this.http.get(this._baseUrl + "customerwithcustomfields/" + id);
  }

  getSpecifiedCustomer(id: number)
  {
    return this.http.get(this._baseUrl + "customer/" + id);
  }

  addNewActivity(newActivity: any) {
    return this.http.post(this._baseUrl + "activities", newActivity);
  }

  addNewActivityWithNextStep(newActivityWithNextStep: any) {
    return this.http.post(this._baseUrl + "activitynextstep", newActivityWithNextStep);
  }

  // Create next step
  addNewNextStep(nextstep: any) {
    return this.http.post(this._baseUrl + "nextstep", nextstep);
  }

  // Customersida
  getContactpersonsByCustomerId(id: number) {
    return this.http.get(this._baseUrl + "contactpersons/" + id);
  }

  addNewContactperson(contact: any) {
    return this.http.post(this._baseUrl + "contactperson/", contact);
  }

  addNewCustomer(customer: any) {
    return this.http.post(this._baseUrl + "customer", customer);
  }

  maskContactperson(contact: any) {
    return this.http.put(this._baseUrl + "maskcontactperson/", contact);
  }

  editContactperson(contact: any) {
    return this.http.put(this._baseUrl + "contactperson/", contact);
  }

  changeResponsibleContact(contact: any) {
    return this.http.put(this._baseUrl + "responsiblecontactperson/", contact);
  }

  getAllProjectsForCustomer(customer: number) {      //! FLYTTA TILL PROJECTSERVICE NÄR DEN BLIR KLAR 
    return this.http.get('http://localhost:5000/api/project/projectsforcustomer/' + customer);
  }

  getAllActivitiesForCustomer(customer: number) {
    return this.http.get(this._baseUrl + "activities/" + customer + "/false");
  }

  getArchivedActivitiesForCustomer(customer: number) {
    return this.http.get(this._baseUrl + "activities/" + customer + "/true");
  }
  
  getAllActivitiesForOrganization() { //DESSA
    return this.http.get(this._baseUrl + "allactivities/false");
  }

  getArchivedActivitiesForOrganization() { //DESSA
    return this.http.get(this._baseUrl + "allactivities/true");
  }

  addNewCustomField(customField: any) {
    return this.http.post(this._baseUrl + "customfield", customField);
  }

  deleteCustomField(customField: any) {
    return this.http.delete(this._baseUrl + "customfield/" + customField);
  }

  editCustomer(customer: any) {
    return this.http.put(this._baseUrl + "customer/", customer);
  }

  archiveActivity(activityId: number) {
    return this.http.put(this._baseUrl + "archiveactivity", activityId);
  } 

  unArchiveActivity(activityId: number) {
    return this.http.put(this._baseUrl + "unarchiveactivity", activityId);
  }

  getNotificationsForUser() {
    return this.http.get(this._baseUrl + "usernotifications");
  }

  getTodosForUser() {
    return this.http.get(this._baseUrl + "alltodos/false");
  }

  getCheckedTodosForUser() {
    return this.http.get(this._baseUrl + "alltodos/true");
  }

  checkTodo(activityId) {
    return this.http.put(this._baseUrl + "checktodo/" + activityId, null);
  }

  unCheckTodo(activityId) {
    return this.http.put(this._baseUrl + "unchecktodo/" + activityId, null);
  }
}
