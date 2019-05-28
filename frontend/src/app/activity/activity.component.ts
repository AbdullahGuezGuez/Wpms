import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { MatPaginator, MatTableDataSource, MatSortModule, MatSort, MatFormFieldModule, MatInputModule } from '@angular/material';
import { map } from 'rxjs/operators';
import { Activity } from '../_models/activity';
import { CustomerService } from '../_services/customer.service';
import { ActivatedRoute, Route, Router } from '@angular/router';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: "app-activity",
  templateUrl: "./activity.component.html",
  styleUrls: ["./activity.component.css"]
})
export class ActivityComponent implements OnInit {
  id: number;
  displayedColumns: string[] = ["date"];
  dataSource = new MatTableDataSource<Activity>();
  noData = this.dataSource.connect().pipe(map(data => data.length === 0));
  activity: any = {};
  showArchived: boolean = false;

  @ViewChild(MatSort) sort: MatSort;
  @ViewChild(MatPaginator) paginator: MatPaginator;

  constructor(
    public _authService: AuthService, 
    private _customerService: CustomerService,
    private _activatedroute: ActivatedRoute,
    private _router: Router
  ) {}

  ngOnInit() {
    this.GetCurrentCustomer();
    this.GetAllActivities();
  }

  ngAfterViewInit() {
    this.dataSource.sort = this.sort;
    this.dataSource.paginator = this.paginator;
  }

  GetCurrentCustomer() {
    this._activatedroute.params.subscribe(params => {
      this.id = params["id"];
    });
  }

  GetAllActivities() {
    this.showArchived = false;
    this._customerService
      .getAllActivitiesForCustomer(this.id)
      .subscribe(res => {
        this.dataSource.data = res as Activity[];
      });
  }

  GetArchivedActivities() {
    this.showArchived = true;
    this._customerService
      .getArchivedActivitiesForCustomer(this.id)
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
