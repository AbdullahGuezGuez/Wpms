import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/_models/user';
import { AdminService } from 'src/app/_services/admin.service';
import { RolesModalComponent } from '../roles-modal/roles-modal.component';
import { BsModalService, BsModalRef } from 'ngx-bootstrap';
import { AuthService } from 'src/app/_services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit {

  users: any;
  aUser: any;
  bsModalRef: BsModalRef;
  availableRoles: any;
  valueHolder: any;
  isSystemAdmin: boolean = false;
  organizations: any;
  usersOrganizations: any;


  constructor(public _authService: AuthService, private adminService: AdminService, private modalService: BsModalService, private _router: Router) { }

  ngOnInit() {
    this.getUsersWithRoles();
    this.adminCheck();
    this.getAllOrganizations();
  }

  async getUsersWithRoles() {
    await this.delay(500);
    this.adminService.getUsersWithRoles().subscribe((users: User[]) => {
      this.users = users;
    }, error => {
      console.log(error);
    });
  }

  getAllOrganizations() {
    this.adminService.getAllOrganizations().subscribe(data => this.organizations = data);
  }

  updateUsersOrganizations(values: any) {
    console.log(values);
  }

  organizationClick(userId: number) {
    this.adminService.getUsersOrganizationById(userId).subscribe(data => this.usersOrganizations = data);
    console.log(userId);
    for (let i = 0; i < this.users.length; i++) 
    {
      if(this.users[i].id == userId)
      {
        this.aUser = this.users[i];
      }
    }
  }

  editRolesModal(user) {
    const initialState = {
      user,
      roles: this.getRolesArray(user)
    };
    this.bsModalRef = this.modalService.show(RolesModalComponent, {initialState});
    
    this.bsModalRef.content.updateSelectedRoles.subscribe((values) => {
      const rolesToUpdate = {
        roleNames: [...values.filter(el => el.checked === true).map(el => el.name)]
      };
      if (rolesToUpdate) {
        this.adminService.updateUserRoles(user.username, rolesToUpdate).subscribe(() => {
          user.roles = [...rolesToUpdate.roleNames];
        }, error => {
          console.log(error);
        });
        this.getUsersWithRoles();
      }
        
    });

  }

  private getRolesArray(user) {
    const roles = [];
    const userRoles = user.roles;

    this.adminService.getAllRoles().subscribe(data => {
      this.availableRoles = data;
    }, error => console.log(error),
    () => {
      for (let i = 0; i < this.availableRoles.length; i++) {
        let isMatch = false;

        for (let j = 0; j < userRoles.length; j++) {
          if (this.availableRoles[i].name === userRoles[j].name) {
            isMatch = true;
            this.availableRoles[i].checked = true;
            roles.push(this.availableRoles[i]);
            break;
          }
        }
        if (!isMatch) {
          this.availableRoles[i].checked = false;
          roles.push(this.availableRoles[i]);
        }
      }
    });
    return roles;
  }

  async maskUser(id)
  {
    this.adminService.maskUser(id).subscribe();
    await this.delay(1000);
    this.getUsersWithRoles();

  }

  resetPassword(id)
  {
    this.adminService.resetPassword(id).subscribe();
  }

  toRoleManagement()
  {
    this._router.navigateByUrl("rolemanagement");
  }

  async adminCheck()
  {
    this._authService.isSystemAdmin().subscribe(data => this.valueHolder = data);
    await this.delay(200);
    if(this.valueHolder == 1)
    {
      this.isSystemAdmin = true;
    }
    else
    {
      this.isSystemAdmin = false;
    }
  }

  private delay(ms: number)
  {
  return new Promise(resolve => setTimeout(resolve, ms));
  }

  
}
