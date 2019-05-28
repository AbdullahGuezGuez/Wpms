import { Component, OnInit, ViewChild, AfterViewInit, Input } from '@angular/core';
import { MatPaginator, MatTableDataSource, MatSortModule, MatSort, MatFormFieldModule, MatInputModule } from '@angular/material';
import { map } from 'rxjs/operators';
import { Activity } from '../../_models/activity';
import { CustomerService } from '../../_services/customer.service';
import { ActivatedRoute, Route, Router } from '@angular/router';
import { AuthService } from '../../_services/auth.service';

@Component({
  selector: 'app-all-activities',
  templateUrl: './all-activities.component.html',
  styleUrls: ['./all-activities.component.css']
})
export class AllActivitiesComponent implements OnInit {
  id: number;
  displayedColumns: string[] = ["date"];
  dataSource = new MatTableDataSource<Activity>();
  noData = this.dataSource.connect().pipe(map(data => data.length === 0));
  activity: any = {};
  showArchived: boolean = false;
  showChecked: boolean = false;
  url: any;

  @ViewChild(MatSort) sort: MatSort;
  @ViewChild(MatPaginator) paginator: MatPaginator;

  constructor(
    public _authService: AuthService, 
    private _customerService: CustomerService,
    private _activatedroute: ActivatedRoute,
    private _router: Router
  ) {}

  ngOnInit() {
    this.checkUrl();
    this.GetAllActivities();
  }
  
  ngAfterViewInit() {
    this.dataSource.sort = this.sort;
    this.dataSource.paginator = this.paginator;
  }
  
  checkUrl() {
    this.url = this._router.url;
  }

  GetAllActivities() {
    if(this.url == '/allactivities')
    {
    this.showArchived = false;
    this._customerService
      .getAllActivitiesForOrganization()
      .subscribe(res => {
        this.dataSource.data = res as Activity[];
      });
    }
    else if(this.url == '/todos')
    {
      this.showChecked = false;
      this._customerService
      .getTodosForUser()
      .subscribe(res => {
        this.dataSource.data = res as Activity[];
      });
    }
    
  }

  GetArchivedActivities() {
    this.showArchived = true;
    this._customerService
      .getArchivedActivitiesForOrganization()
      .subscribe(res => {
        this.dataSource.data = res as Activity[];
      });
  }

  GetCheckedTodos() {
    this.showChecked = true;
    this._customerService
      .getCheckedTodosForUser()
      .subscribe(res => {
        this.dataSource.data = res as Activity[];
      });
  }

  public doFilter = (value: string) => {
    this.dataSource.filter = value.trim().toLocaleLowerCase();
  };

  activityClick(activityId: number) {
    for (let i = 0; i < this.dataSource.data.length; i++) 
    {
      if(this.dataSource.data[i].id == activityId)
      {
        this.activity = this.dataSource.data[i];
      }
    }
  }

  createNextStep(activityId) {
    this._router.navigateByUrl('createnextstep/' + activityId + "/" + this.id);
  }

  async archiveActivity(activityId: any) {
    this._customerService.archiveActivity(activityId).subscribe();
    await this.delay(500);
    this.GetAllActivities();
  }

  async unArchiveActivity(activityId: any) {
    this._customerService.unArchiveActivity(activityId).subscribe();
    await this.delay(500);
    this.GetAllActivities();
  }

  async markTodoDone(activityId: any) {
    document.getElementById("closeActivityModal").click();
    this._customerService.checkTodo(activityId).subscribe();
    await this.delay(500);
    this.GetAllActivities();
  }

  async markTodoNotDone(activityId: any) {
    document.getElementById("closeActivityModal").click();
    this._customerService.unCheckTodo(activityId).subscribe();
    await this.delay(500);
    this.GetCheckedTodos();
  }

  leftClick() {
    console.log("Left Click");
  }

  rightClick() {
    console.log("Right Click");
  }

  private delay(ms: number) {
    return new Promise(resolve => setTimeout(resolve, ms));
  }
}