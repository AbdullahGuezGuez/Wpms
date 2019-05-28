import { Component, OnInit, AfterViewInit, Input, ViewChild, ElementRef } from '@angular/core';
import { CustomerService } from 'src/app/_services/customer.service';
import { AmazingTimePickerService } from 'amazing-time-picker';
import { AuthService } from 'src/app/_services/auth.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-createnextstep',
  templateUrl: './createnextstep.component.html',
  styleUrls: ['./createnextstep.component.css']
})
export class CreatenextstepComponent implements OnInit, AfterViewInit {
  model: any = {};
  types: string[] = ['Telephone', 'Email', 'Meeting', 'ToDo'];
  businessParticipants: any = [];
  customerParticipants: any = [];
  customer: any = {};
  activityId: any;
  customerId: any;

  myFilter = (d: Date): boolean => {
    const day = d.getDay();
    // Prevent Saturday and Sunday from being selected.
    return day !== 0 && day !== 6;
  }

  constructor(private _authService: AuthService, private _customerService: CustomerService, private _atp: AmazingTimePickerService, private _activatedroute: ActivatedRoute) { }

  ngOnInit() {
    this.getActivity();
    this.getContactpersons();
  }

  ngAfterViewInit() {
    this.getAllUsers();    
  }

  getActivity() {
    this._activatedroute.params.subscribe(params => { 
      this.activityId = params['actId']; 
      this.customerId = params['custId'];
    });
  }

  getAllUsers() {
    this._customerService.getAllUsersInOrganization().subscribe(data => this.businessParticipants = data);
  }

  getContactpersons() {
    this._customerService.getContactpersonsByCustomerId(this.customerId).subscribe(data => this.customerParticipants = data);
  }

  getAllContactsForSelectedCustomer(customerId) { // Hämtar alla kontaktpersoner för en viss kund
    this._customerService.getContactpersonsByCustomerId(customerId).subscribe(data => this.customerParticipants = data);
  }

  setDate(date) {
    const amazingTimePicker = this._atp.open({
      theme: 'dark',
    });
    amazingTimePicker.afterClose().subscribe(time => {
      this.model.time = time;
    });
  }

  addNextstep(value) {
    value.activityId = this.activityId;
    value.customerId = this.customerId;
    this._customerService.addNewNextStep(value).subscribe();

  }
}
