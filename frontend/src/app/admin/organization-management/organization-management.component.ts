import { Component, OnInit } from '@angular/core';
import { AdminService } from 'src/app/_services/admin.service';
import { AuthService } from 'src/app/_services/auth.service';

@Component({
  selector: 'app-organization-management',
  templateUrl: './organization-management.component.html',
  styleUrls: ['./organization-management.component.css']
})
export class OrganizationManagementComponent implements OnInit {
  organizations: any;
  model: any = {};
  updateModel: any = {};

  constructor(public _authService: AuthService, private _adminService: AdminService) { }

  ngOnInit() {
    this.getOrganizations();
  }

  getOrganizations() {
    this._adminService.getAllOrganizations()
      .subscribe(data => this.organizations = data);
  }

  async createNewOrganization() {
    this._adminService.postNewOrganization(this.model).subscribe(
      () => console.log('Posted new Organization')
    );
    await this.delay(1000);
    this.getOrganizations();
  }

  async organizationToDelete() {
    const id = (document.getElementById('organizationToDelete') as HTMLInputElement).value;
    this._adminService.deleteOrganization(id);
    await this.delay(1000);
    this.getOrganizations();
  }

  async updateOrganization() {
    const id = (document.getElementById('organizationToUpdate') as HTMLInputElement).value;
    this._adminService.updateOrganization(id, this.updateModel);
    await this.delay(1000);
    this.getOrganizations();
  }

  private delay(ms: number)
    {
      return new Promise(resolve => setTimeout(resolve, ms));
    }

}
