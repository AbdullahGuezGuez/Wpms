import { Component, OnInit, AfterViewInit } from '@angular/core';
import { AuthService } from '../app/_services/auth.service';
import { Router } from '@angular/router';
import { OrgUser } from './_models/orgUser';
import { AdminService } from './_services/admin.service';
import { ProjectService } from './_services/project.service';
import { CustomerService } from './_services/customer.service';
import { startTimeRange } from '@angular/core/src/profile/wtf_impl';


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit, AfterViewInit {
  private _router: Router;
  usersOrganizations: any = [];
  isSystemAdmin: boolean = false;
  valueHolder: any;
  loggedInUser: any = {};
  notifications: any;
  activity: any = {};
  updatingTrello: boolean = false;

  timeLeft: number = 5;
  interval;


  constructor(public _authService: AuthService, private router: Router, private _adminservice: AdminService, private _projectservice: ProjectService, private _customerService: CustomerService) 
  { 
    this._router = router;
  }

  ngOnInit() {
    this.adminCheck();
    if(!this.isSystemAdmin)
    {
      this.getNotificationsForUser();
      this.startTimer();
    }
    this.getLoggedInUser();
  }
  
  ngAfterViewInit() {
    
  }
  
  async deactivateUpdateTrelloTimer()
  {
    this.updatingTrello = true;
    await this.delay(30000);
    this.updatingTrello = false;
  }

  startTimer() {
    this.interval = setInterval(() => {
      if(this.timeLeft > 0) {
        this.timeLeft--;
      } else {
        this.getNotificationsForUser();
        this.timeLeft = 300;
      }
    },1000)
  }

  pauseTimer() {
    clearInterval(this.interval);
  }

  getNotificationsForUser() {
    this._customerService.getNotificationsForUser().subscribe(data => {
      this.notifications = data; // TODO: Ta bort
    });
  }

  activityClick(activityId: number) {
    for (let i = 0; i < this.notifications.usersActivities.length; i++) 
    {
      if(this.notifications.usersActivities[i].id == activityId)
      {
        this.activity = this.notifications.usersActivities[i];
      }
    }
  }

  goToCreateNewActivity() {
    this._router.navigateByUrl('createactivity');
  }

  private signOut() 
  {
    localStorage.clear()
    this._router.navigateByUrl('');
  }

  getLoggedInUser() {
    this._authService.getLoggedInUser().subscribe(data => this.loggedInUser = data);
  }

  showOrganizations()
  {
    this._adminservice.getUsersOrganizationsToken()
    .subscribe(data => this.usersOrganizations = data);
  }

  changeOrg(orgId: number) 
  {
    this._authService.changeOrg(orgId).subscribe();
    location.reload();
  }

  refreshTrello()
  {
    this._projectservice.updateTrelloData().subscribe();
    this.deactivateUpdateTrelloTimer();
  }

  async adminCheck() {
    this._authService.isSystemAdmin().subscribe(data => {
      this.valueHolder = data;
      if (this.valueHolder == 1) {
        this.isSystemAdmin = true;
      }
      else {
        this.isSystemAdmin = false;
      }
    });
    await this.delay(500);
  }

  async markTodoDone(activityId: any) {
    document.getElementById("closeActivityModal").click();
    this._customerService.checkTodo(activityId).subscribe();
    await this.delay(500);
    this.getNotificationsForUser();
  }

  async markTodoNotDone(activityId: any) {
    document.getElementById("closeActivityModal").click();
    this._customerService.unCheckTodo(activityId).subscribe();
    await this.delay(500);
    this.getNotificationsForUser();
  }

  private delay(ms: number)
    {
      return new Promise(resolve => setTimeout(resolve, ms));
    }

}
