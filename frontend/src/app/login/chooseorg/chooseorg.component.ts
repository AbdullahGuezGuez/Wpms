import { Component, OnInit, Input } from '@angular/core';
import { AdminService } from 'src/app/_services/admin.service';
import { AuthService } from 'src/app/_services/auth.service';
import { OrgUser } from 'src/app/_models/orgUser';

@Component({
  selector: 'app-chooseorg',
  templateUrl: './chooseorg.component.html',
  styleUrls: ['./chooseorg.component.css']
})
export class ChooseorgComponent implements OnInit { 
  organizations: any = [];
  @Input() userDetails: OrgUser;

  constructor(private _adminService: AdminService, private _authService: AuthService) { }

  ngOnInit() {
    this._adminService.getUsersOrganizations(this.userDetails.username)
    .subscribe(data => this.organizations = data);
  }

  loginWithOrg(orgId: number) {
    this._authService.secondLogin(this.userDetails, orgId).subscribe(next => {
      console.log('Logged in successfully');
      location.reload();
    }, error => {
      console.log('Failed to login');
    });

  }

}
