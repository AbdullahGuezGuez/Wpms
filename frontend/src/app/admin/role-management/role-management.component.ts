import { Component, OnInit } from '@angular/core';
import { AdminService } from 'src/app/_services/admin.service';
import { analyzeAndValidateNgModules } from '@angular/compiler';
import { throwToolbarMixedModesError } from '@angular/material';
import { AuthService } from 'src/app/_services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-role-management',
  templateUrl: './role-management.component.html',
  styleUrls: ['./role-management.component.css']
})
export class RoleManagementComponent implements OnInit {
  roleModel: any = {};
  dbRoles: any;
  dbClaims: any;
  dbRoleWithClaims: any;
  choosenRole: any = {};
  choosenRoleToShow: any = [];
  claimsToShow: any;
  roleInfoUsers: any;
  roleInfoName: any;

  constructor(public _authService: AuthService, private _adminService: AdminService, private _router: Router) { }

  ngOnInit() {
    this.getAllRoles();
    this.getAllRoleWithClaims();
    this.getAllClaims();
  }

  getAllRoles() {
    this._adminService.getAllRoles()
      .subscribe(data => this.dbRoles = data);
  }

  getAllRoleWithClaims() {
    this._adminService.getAllRoleWithClaims()
      .subscribe(data => this.dbRoleWithClaims = data)
  }

  getAllClaims() {
    this._adminService.getAllClaims()
      .subscribe(data => this.dbClaims = data)
  }

  async addRole() {
    this._adminService.createRole(this.roleModel).subscribe();
    await this.delay(500);
    this.getAllRoles();
    this.getAllRoleWithClaims();
    this.getAllClaims();
  }

  async roleToDelete() {
    const id = (document.getElementById('roleToDelete') as HTMLInputElement).value;
    this._adminService.deleteRole(id);
    await this.delay(500);
    this.getAllRoles();
    this.getAllRoleWithClaims();
    this.getAllClaims();
  }

  changeClaimsOnClick(role : any) {
    for (let i = 0; i < this.dbRoleWithClaims.length; i++) 
    {
      if(this.dbRoleWithClaims[i].name == role)
      {
          this.choosenRoleToShow.Name = role;
          this.choosenRoleToShow.Id = this.dbRoleWithClaims[i].id;
          this.claimsToShow = this.dbRoleWithClaims[i].claimsWithBool;
          document.getElementById(this.dbRoleWithClaims[i].id).setAttribute("class","selectedItem");
      }
      else
      {
        document.getElementById(this.dbRoleWithClaims[i].id).removeAttribute("class");
      }
    }
  }

  saveClaimChanges() {
    this.choosenRole.ClaimsWithBool = this.claimsToShow;
    this.choosenRole.Name = this.choosenRoleToShow.Name;
    this.choosenRole.Id = this.choosenRoleToShow.Id;
    this._adminService.changeRoleClaims(this.choosenRole);
  }

  changeClaimBoolOnClick(claim: any, event: any) {
     for (let i = 0; i < this.claimsToShow.length; i++) 
    {
      if(this.claimsToShow[i].claimValue == claim.claimValue)
      {
          this.claimsToShow[i].hasClaim = event.currentTarget.checked;
      }
    }
  }

  roleInfo(roleId :any, roleName :any) {
    this._adminService.getUsersOnRole(roleId)
    .subscribe(data => this.roleInfoUsers = data);
    this.roleInfoName = roleName;
 }

 backToUsers()
  {
    this._router.navigateByUrl("users");
  }

  private delay(ms: number)
  {
  return new Promise(resolve => setTimeout(resolve, ms));
  }
}
