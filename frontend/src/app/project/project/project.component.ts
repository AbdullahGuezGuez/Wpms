import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClient, HttpRequest, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { log } from 'util';
import { Router } from '@angular/router';
import { ProjectService } from '../../_services/project.service';
import { AdminService } from '../../_services/admin.service';
import { ShowOnDirtyErrorStateMatcher, MatDialog } from '@angular/material';
import { ActivatedRoute } from '@angular/router';
import { DragDropModule, moveItemInArray, CdkDragDrop, transferArrayItem } from '@angular/cdk/drag-drop';
import {TaskListModalComponent} from '../task-List-Modal/task-List-Modal.component';
import { AuthService } from 'src/app/_services/auth.service';

@Component({
  selector: 'app-project',
  templateUrl: './project.component.html',
  styleUrls: ['./project.component.css']
})
export class ProjectComponent implements OnInit {

  projects:any = [];
  sortedProjects: any;
  project: any;
  model: any = {};
  id: any;
  usersToProject: any = [];
  projectId: any; //! ANVÄNDS TILL ?
  users: any=[];
  toggle = true; //! ANVÄNDS TILL ?
  targetedProjectId: any;
  userTaskList: any;
  boardLists: any = [];
  personalTasks: any;
  active: boolean;
  waiter: boolean = false;

  constructor(public _authService: AuthService, public dialog: MatDialog, private _http: HttpClient, private _projectService: ProjectService, 
    private _adminService: AdminService, private _activatedRoute: ActivatedRoute) { }

  ngOnInit() {
      this.getStartProject();
      this.getAllUsers()
      this.getAllProjects();  
      this.showProjectMembers();
      this.getBoardLists();
  }

  
  getAllProjects(){
    this._adminService.getAllProjects().subscribe(data => {
      this.projects = data;
      this.projects.sort((n1, n2)=> {
        if(n1.priority > n2.priority) {
          return 1;
        } else if(n1.priority < n2.priority) {
          return -1;
        } else {
          return 0;
        }
      });
    });
  }

  async changePriority(value) {
    value.ProjectId = this.id;
    value.OldPrio = this.project.priority;
    this._projectService.changePriority(value).subscribe();
    await this.delay(500);
    document.getElementById('closePrioChange').click();
    this.getStartProject();
    this.getAllProjects();
  }

  getOneProject(id: string){
    this._adminService.getOneProject(id).subscribe(data => {this.project = data, this.active = this.project.active});
    this.id = id;
    this.toggle = !this.toggle;//! ANVÄNDS TILL ?
    this._projectService.getAllCurrentProjectUsers(this.id).subscribe(data => this.usersToProject = data);
    this._projectService.getAllProjectUsers(this.id).subscribe(data => this.users = data);
    this.projectId = this.id; //! ANVÄNDS TILL ?
    this.getBoardLists();
    this.userTaskList = [];
  }

  getStartProject(){
    this._activatedRoute.params.subscribe(params => { this.id = params['id']; });
    this._adminService.getOneProject(this.id).subscribe(data => {this.project = data, this.active = this.project.active});
  }

  showProjectMembers() {
  this._projectService.getAllCurrentProjectUsers(this.id).subscribe(data => this.usersToProject = data);
  this._projectService.getAllProjectUsers(this.id).subscribe(data => this.users = data);
  }

  getAllUsers() {
    this._adminService.getAllUsers()
        .subscribe(data => this.users = [data]);
    }

  drop(event: CdkDragDrop<string[]>) {
    if (event.previousContainer === event.container) {
      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
    } else {
      transferArrayItem(event.previousContainer.data,
                        event.container.data,
                        event.previousIndex,
                        event.currentIndex);
    }
  }

  addMembersToProject() {
    if (this.usersToProject.length < 1 ) {
    } 
    else {
      this._projectService.addUsersToProject(this.id, this.usersToProject).subscribe();
    }
  }

  getBoardLists(){
    this._projectService.getBoardLists(this.id).subscribe(data => this.boardLists = data);
  }

  getUserTasks(e: any){
    const idFromList = e.target.value;
    this._projectService.getUserTasks(this.id, idFromList).subscribe(data => this.userTaskList = data);
  }

  changeProjectStatus()
  {
    this._projectService.changeActiveBool(this.project.id).subscribe();
    this.loader();
  }

  async loader()
  {
    this.waiter = true;
    await this.delay(1000);
    this.active = !this.active;
    this.waiter = false;
  }

  async changeProjectName()
  {
    // this.customer.customer.name = this.model.name;
    // this._customerService.editCustomer(this.customer.customer).subscribe();
    // document.getElementById("customerName").click();
    // await this.delay(500);
    // this.GetCustomer();
    // this.model = {};
  }

  private delay(ms: number)
  {
    return new Promise(resolve => setTimeout(resolve, ms));
  }

  getTargetedUserTasks(value: any){
    this.personalTasks = value;
    this.openPersonalTasks();
  }
  
  // getTargetedUserTasks(user)
  openPersonalTasks(): void {
    let dialogRef = this.dialog.open(TaskListModalComponent,  {
  data: {
    datakey: this.personalTasks.cardNames
  }
    });
  }
  
}
