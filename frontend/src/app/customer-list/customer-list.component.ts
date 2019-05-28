import { Component, OnInit, ViewChild, ChangeDetectionStrategy, AfterViewInit } from "@angular/core";
import { MatPaginator, MatTableDataSource, MatSortModule, MatSort, MatFormFieldModule, MatInputModule } from "@angular/material";
import { Observable } from "rxjs";
import { map } from "rxjs/operators";
import { Customer } from "../_models/customer";
import { CustomerService } from "../_services/customer.service";
import { Router } from "@angular/router";
import { AuthService } from '../_services/auth.service';

@Component({
  selector: "app-customer-list",
  templateUrl: "./customer-list.component.html",
  styleUrls: ["./customer-list.component.css"]
})
export class CustomerListComponent implements OnInit, AfterViewInit {
  displayedColumns: string[] = ["name", "region", "org.nr", "responsible", "status"];
  dataSource = new MatTableDataSource<Customer>();
  noData = this.dataSource.connect().pipe(map(data => data.length === 0));
  model: any = {};
  waiter: boolean = false;
  showActive: boolean = true;

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  constructor(
    public _authService: AuthService, 
    private _customerService: CustomerService,
    private _router: Router
  ) { }

  ngOnInit() {
    this.GetAllCustomerForOrganization();
    
  }
  ngAfterViewInit() {
    this.dataSource.sort = this.sort;
    this.dataSource.paginator = this.paginator;
  }

  removeData() {
    //! används till ?
    const data = this.dataSource.data.slice();
    data.shift();
    this.dataSource.data = data;
  }

  public doFilter = (value: string) => {
    this.dataSource.filter = value.trim().toLocaleLowerCase();
  };

  GetAllCustomerForOrganization() {
    this._customerService.getAllCustomersForOrganization().subscribe(res => {
      this.dataSource.data = res as Customer[];
    });
  }

  GetAllArchivedCustomerForOrganization() {
    this._customerService.getAllInactiveCustomersForOrganization().subscribe(res => {
      this.dataSource.data = res as Customer[];
    });
    this.loader();
  }

  GetAllUnarchivedCustomerForOrganization() {
    this._customerService.getAllCustomersForOrganization().subscribe(res => {
      this.dataSource.data = res as Customer[];
    });
    this.loader();
  }

  SelectedCustomer(customer) {
    for (let i = 0; i < this.dataSource.data.length; i++) {
      if (this.dataSource.data[i].name == customer) {
        this._router.navigateByUrl("customer/" + this.dataSource.data[i].id);
      }
    }
  }

  addCustomer() {
    this._customerService.addNewCustomer(this.model).subscribe(res => {
      document.getElementById("closeCustomerModal").click();
      this._router.navigateByUrl("customer/" + (res));
    });
  }

  async loader()
  {
    this.waiter = true;
    await this.delay(1000);
    this.showActive = !this.showActive;
    this.waiter = false;
  }

  private delay(ms: number)
  {
    return new Promise(resolve => setTimeout(resolve, ms));
  }
}
