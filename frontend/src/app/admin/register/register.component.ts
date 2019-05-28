import { Component, OnInit } from '@angular/core';
import { AuthService} from 'src/app/_services/auth.service';
import { AdminService } from 'src/app/_services/admin.service';
import { CheckPasswordDirectiveService } from 'src/app/_services/CheckPasswordDirective.service';
@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  selectedRoles: any;
  model: any = {};
  roles: any;
  isSystemAdmin: boolean = false;
  valueHolder: any;
  organizations: any;
  infoText: any;
  orgId: any;
 

  constructor(public _authService: AuthService, private adminService: AdminService) {}

  ngOnInit() {
    this.getAllRoles();
    this.selectedRoles = [];
    this.adminCheck();
  }

  getAllRoles() {
   this.adminService.getRoles().subscribe(data => {
     this.roles = data;
   });
  }

  register() {
    if(this.isSystemAdmin)
    {
      this.model.organizationId = (document.getElementById('organizations') as HTMLInputElement).value;
    }
    this.adminService.register(this.model).subscribe(res => {},
      error => {
        console.log(error);
        if(error.error.text == "Success")
        {
          this.infoText = this.model.Username + " has been created!"
          this.clearAllFields();
        }
        else
        {
          this.infoText = error.error;
        }
        });
  }

  clearAllFields()
  {
    this.model = {};
  }


  getOrganizations() {
    this.adminService.getAllOrganizations()
      .subscribe(data => this.organizations = data);
  }


  async adminCheck()
  {
    this._authService.isSystemAdmin().subscribe(data => this.valueHolder = data);
    await this.delay(200);
    if(this.valueHolder == 1)
    {
      this.isSystemAdmin = true;
      this.getOrganizations();
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
