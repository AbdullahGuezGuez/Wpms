import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class ProjectService {

  private BaseUrl = 'https://wpms.azurewebsites.net/api/';

  constructor(public http: HttpClient) { }


  createProject(project: any) {
    return this.http.post(this.BaseUrl + 'createproject', project);
  }

  getAllProjectUsers(id: any) {
    return this.http.get(this.BaseUrl + 'projectmembers/' + id);
  }

  getAllCurrentProjectUsers(id: any) {
    return this.http.get(this.BaseUrl + 'projectmembersinproject/' + id);
  }

  addUsersToProject(id: any, users: any) {
    return this.http.post(this.BaseUrl + 'createprojectusers/' + id, users);
  }

  updateTrelloData() {
    return this.http.get(this.BaseUrl + 'trello/updatetrellodata');
  }

  getAllProjects() {
    return this.http.get(this.BaseUrl + 'projectmembers/' + 'projects');
  }

  getUserTasks(projectId: any, listId: any) {
    return this.http.get(this.BaseUrl + 'getUserTasks/' + projectId + "/" + listId);
  }

  getProjectValues(id: any) {
    return this.http.get(this.BaseUrl + 'projectvalues/' + id)
  }

  getBoardLists(Id: any) {
    return this.http.get(this.BaseUrl + 'boardlists/' + Id);
  }

  getAvaliableBoards() {
    return this.http.get(this.BaseUrl + 'availableboards');
  }

  changeTrelloBoardConnection(values: any) {
    return this.http.put(this.BaseUrl + 'changetrelloboard', values);
  }

  removeTrelloBoardConnection(id: any) {
    return this.http.put(this.BaseUrl + 'removetrelloconnection', id);
  }

  createTrelloBoard(values: any) {
    return this.http.post(this.BaseUrl + 'createtrelloboard', values);
  }

  changeActiveBool(value: any) {
    return this.http.put(this.BaseUrl + 'changeactiveproject', value);
  }

  // Projectsidan
  changePriority(values: any) {
    return this.http.put(this.BaseUrl + 'changepriority', values);
  }

  getDashboardProjects() {
    return this.http.get(this.BaseUrl + 'dashboardprojects');
  }

}

