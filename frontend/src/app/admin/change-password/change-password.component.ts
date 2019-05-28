import { Component, OnInit } from '@angular/core';
import { CheckPasswordDirectiveService } from 'src/app/_services/CheckPasswordDirective.service';
import { AdminService } from 'src/app/_services/admin.service';

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.css']
})
export class ChangePasswordComponent implements OnInit {
  model: any = {};
  infoText: any;

  constructor(private _adminservice: AdminService) { }

  ngOnInit() {
  }

  changePassword()
  {
    console.log(this.model);
    this._adminservice.changePassword(this.model).subscribe(res => {},
      error => {
        console.log(error);
        if(error.error.text == "Success")
        {
          this.infoText = "Password has been changed!"
          this.model = {};
        }
        else
        {
          this.infoText = error.error;
        }
        });
  }

}
