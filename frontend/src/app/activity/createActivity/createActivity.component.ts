import { Component, OnInit, AfterViewInit, Input, ElementRef, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CustomerService } from '../../_services/customer.service';
import { MatListModule } from '@angular/material/list';
import { MatDatepickerModule, MatInputModule, MatNativeDateModule } from '@angular/material';
import { AmazingTimePickerService } from 'amazing-time-picker';
import { AuthService } from 'src/app/_services/auth.service';

@Component({
  selector: 'app-createActivity',
  templateUrl: './createActivity.component.html',
  styleUrls: ['./createActivity.component.css']
})
export class CreateActivityComponent implements OnInit, AfterViewInit {
  model: any = {};
  customers: any;
  types: string[] = ['Telephone', 'Email', 'Meeting', 'ToDo'];
  businessParticipants: any = [];
  customerParticipants: any = [];
  includeNextStep: boolean = false;
  customer: any = {};
  isCustomer: boolean = false; 

  @Input() customerId: number = 0;

  myFilter = (d: Date): boolean => {
    const day = d.getDay();
    // Prevent Saturday and Sunday from being selected.
    return day !== 0 && day !== 6;
  }

  constructor(public _authService: AuthService, private _activatedroute: ActivatedRoute, private _customerService: CustomerService, private _router: Router, private _atp: AmazingTimePickerService) { }

  ngOnInit() {
    this.checkIfOnCustomerComponent();
  }
  
  ngAfterViewInit() {
    this.getAllCustomers();
    this.getAllUsers();
  }

  async checkIfOnCustomerComponent() {
    if(this.customerId == 0)
    {
      this.isCustomer = false;
    }
    else if(this.customerId > 0)
    {
      this.isCustomer = true;
      this._customerService.getSpecifiedCustomer(this.customerId).subscribe(data => {
        this.customer = data;
      });
      await this.delay(800);
      this.getAllContactsForSelectedCustomer(this.customer.id);
    }
  }

  getAllCustomers() {  
    this._customerService.getAllCustomersForOrganization().subscribe(data => this.customers = data);
  }

  getAllUsers() {
    this._customerService.getAllUsersInOrganization().subscribe(data => this.businessParticipants = data);
  }

  getAllContactsForSelectedCustomer(customerId) { // Hämtar alla kontaktpersoner för en viss kund
    this._customerService.getContactpersonsByCustomerId(customerId).subscribe(data => this.customerParticipants = data);
  }

  addActivity(value: any) {
    console.log(value);
    if(this.isCustomer)
    {
      value.customerId = this.customer.id;
    }
    if(this.includeNextStep)
    {
      this._customerService.addNewActivityWithNextStep(value).subscribe();
    }
    else if (!this.includeNextStep)
    {
      this._customerService.addNewActivity(value).subscribe();
    }
  }

  changeSelected()
  {
    this.includeNextStep = !this.includeNextStep;
  }
  
  setDate(date) {
    const amazingTimePicker = this._atp.open({
      theme: 'dark',
    });
    amazingTimePicker.afterClose().subscribe(time => {
      this.model.time = time;
    });
  }

  setNextStepDate(picker) {
    const amazingTimePicker = this._atp.open({
      theme: 'dark',
    });
    amazingTimePicker.afterClose().subscribe(time => {
      this.model.nextstepTime = time;
    });
  }

  private delay(ms: number) {
    return new Promise(resolve => setTimeout(resolve, ms));
  }
}
