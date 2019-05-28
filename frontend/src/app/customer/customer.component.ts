import { Component, OnInit, ViewChild, ChangeDetectionStrategy, AfterViewInit } from '@angular/core';
import { MatPaginator, MatTableDataSource, MatSortModule, MatSort, MatFormFieldModule, MatInputModule } from '@angular/material';
import { ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs';
import { Xeditable } from 'angular-xeditable';
import { map } from 'rxjs/operators';
import { CustomerService } from '../_services/customer.service';
import { Router } from '@angular/router';
import { Contact } from '../_models/contact';
import { ChartsModule } from 'ng2-charts';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-customer',
  templateUrl: './customer.component.html',
  styleUrls: ['./customer.component.css']
})
export class CustomerComponent implements OnInit, AfterViewInit {
  id: number;
  customer: any;
  displayedColumns: string[] = ['name'];
  dataSource = new MatTableDataSource<Contact>();
  noData = this.dataSource.connect().pipe(map(data => data.length === 0));
  model: any = {};
  contact: any = {};
  projects: any;
  public pieChartLabels = ['Hours Remaining', 'Hours Completed'];
  public pieChartLabelsWithoutTrello = ['No Trello Connection'];
  public pieChartType = 'pie';
  public pieChartColors:{}[]= [ { backgroundColor: ['#F3A09E', '#3ABAAF'] } ];
  public pieChartColorsWithoutTrello:{}[]= [ { backgroundColor: ['#404040'] } ];

  @ViewChild(MatSort) sort: MatSort;

  constructor(public _authService: AuthService, private _activatedroute: ActivatedRoute, private _customerService: CustomerService, private _router: Router) { }

  ngOnInit() {
    this.GetCustomer();
    this.GetAllContacts();
    this.getAllProjects();
  }

  ngAfterViewInit() {
    this.dataSource.sort = this.sort;
  }

  removeData() {          //! används till ?
    const data = this.dataSource.data.slice();
    data.shift();
    this.dataSource.data = data;
  }

  public doFilter = (value: string) => {
    this.dataSource.filter = value.trim().toLocaleLowerCase();
  }

  GetCustomer()
  {
    this._activatedroute.params.subscribe(params => { this.id = params['id']; });
    this._customerService.getCustomerById(this.id).subscribe(data => this.customer = data);
  }

  GetAllContacts() {
    this._customerService.getContactpersonsByCustomerId(this.id).subscribe(res => {
      this.dataSource.data = res as Contact[];
    });
  }

  async getAllProjects()
  {
    this._customerService.getAllProjectsForCustomer(this.id).subscribe(data => this.projects = data);
    await this.delay(500);
  }

  backToCustomerList()
  {
    this._router.navigateByUrl("customerlist");
  }

  SelectedContact(contactId)
  {
      
    for (let i = 0; i < this.dataSource.data.length; i++) 
    {
      if(this.dataSource.data[i].id == contactId)
      {
        this.contact = this.dataSource.data[i];
      }
    }
  }

  async deleteCustomField(customfieldId)
  {
    this._customerService.deleteCustomField(customfieldId).subscribe();
    await this.delay(800);
    this.GetCustomer()
  }

    //!________________________Customer__________________________________________

    async addCustomField()
    {
      this.model.customerId = this.id;
      this._customerService.addNewCustomField(this.model).subscribe();
      await this.delay(500);
      document.getElementById("addCustomFieldBtn").click();
      this.GetCustomer();
      this.model = {};
    }

    async changeCustomerName()
    {
      this.customer.customer.name = this.model.name;
      this._customerService.editCustomer(this.customer.customer).subscribe();
      document.getElementById("customerName").click();
      await this.delay(500);
      this.GetCustomer();
      this.model = {};
    }

    async changeCustomerStatus()
    {
      
      this.customer.customer.customerStatus = this.model.customerStatus;
      this._customerService.editCustomer(this.customer.customer).subscribe();
      document.getElementById("customerStatus").click();
      await this.delay(500);
      this.GetCustomer();
      this.model = {};
    }

    async changeCustomerRegion()
    {
      this.customer.customer.region = this.model.region;
      this._customerService.editCustomer(this.customer.customer).subscribe();
      document.getElementById("region").click();
      await this.delay(500);
      this.GetCustomer();
      this.model = {};
    }

    async changeCustomerAddress()
    {
      this.customer.customer.address = this.model.address;
      this._customerService.editCustomer(this.customer.customer).subscribe();
      document.getElementById("address").click();
      await this.delay(500);
      this.GetCustomer();
      this.model = {};
    }

    async changeCustomerOrganizationNumber()
    {
      this.customer.customer.organizationNumber = this.model.organizationNumber;
      this._customerService.editCustomer(this.customer.customer).subscribe();
      document.getElementById("organizationNumber").click();
      await this.delay(500);
      this.GetCustomer();
      this.model = {};
    }

    async changeCustomerCustomermail()
    {
      this.customer.customer.customermail = this.model.customermail;
      this._customerService.editCustomer(this.customer.customer).subscribe();
      document.getElementById("customermail").click();
      await this.delay(500);
      this.GetCustomer();
      this.model = {};
    }

    async changeCustomerTelephone()
    {
      this.customer.customer.telephone = this.model.telephone;
      this._customerService.editCustomer(this.customer.customer).subscribe();
      document.getElementById("telephone").click();
      await this.delay(500);
      this.GetCustomer();
      this.model = {};
    }

    async changeCustomerCustomerDescription()
    {
      this.customer.customer.customerDescription = this.model.customerDescription;
      this._customerService.editCustomer(this.customer.customer).subscribe();
      document.getElementById("customerDescription").click();
      await this.delay(500);
      this.GetCustomer();
      this.model = {};
    }

//!______________________________Contacts_____________________________

    async addContact()
    {
      this.model.customerId = this.id;
      this._customerService.addNewContactperson(this.model).subscribe();
      await this.delay(1000);
      document.getElementById("closeAddCustomerModal").click();
      this.GetAllContacts();
      this.model = {};
    }

    async maskContact()
    {
      this._customerService.maskContactperson(this.contact.id).subscribe();
      await this.delay(500);
      document.getElementById("closeCustomerModal").click();
      this.GetAllContacts();
    }

    async changeContactName()
    {
      this.contact.name = this.model.name;
      this._customerService.editContactperson(this.contact).subscribe();
      document.getElementById("contactName").click();
      await this.delay(500);
      this.GetAllContacts();
      await this.delay(500);
      this.SelectedContact(this.contact.id);
      this.model = {};
    }

    async changeContactRole()
    {
      this.contact.role = this.model.role;
      this._customerService.editContactperson(this.contact).subscribe();
      document.getElementById("contactRole").click();
      await this.delay(500);
      this.GetAllContacts();
      await this.delay(500);
      this.SelectedContact(this.contact.id);
      this.model = {};
    }

    async changeContactMail()
    {
      this.contact.mail = this.model.mail;
      this._customerService.editContactperson(this.contact).subscribe();
      document.getElementById("contactMail").click();
      await this.delay(500);
      this.GetAllContacts();
      await this.delay(500);
      this.SelectedContact(this.contact.id);
      this.model = {};
    }

    async changeContactTelephone()
    {
      this.contact.telephone = this.model.telephone;
      this._customerService.editContactperson(this.contact).subscribe();
      document.getElementById("contactTelephone").click();
      await this.delay(500);
      this.GetAllContacts();
      await this.delay(500);
      this.SelectedContact(this.contact.id);
      this.model = {};
    }

    async changeResponsibleContact(event: any)
    {
      this.contact.responsible = event.currentTarget.checked;
      this._customerService.changeResponsibleContact(this.contact).subscribe();
      await this.delay(500);
      this.GetAllContacts();
      await this.delay(500);
      this.SelectedContact(this.contact.id);
    }

    private delay(ms: number)
    {
      return new Promise(resolve => setTimeout(resolve, ms));
    }

}
