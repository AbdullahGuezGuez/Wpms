import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { Router } from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt'

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  model: any = {};
  showChooseOrg: boolean = false;
  errorText: any;
  @Output() userLoggedIn = new EventEmitter();

  constructor(private authService: AuthService, private router: Router) { }

  ngOnInit() {
  }

  userLoginIn() {
    this.userLoggedIn.emit('loggedIn');
  }

  async firstLogin() {
    this.authService.FirstLogin(this.model).subscribe(next => {
      console.log('Logged in successfully');
      location.reload();
    }, error => {
      console.log(error.status);
      if (error != null) {
        try {
          var errorName = error.error.text;
          
          if (errorName == "MULTIPLEORGANIZATIONS") {
            this.showChooseOrg = true;
          }
          else {
            this.errorText = "Wrong Username or Password!";
            console.log("Failed to login");
          }
        } catch (e) {
          console.log("Failed to login " + e);
          this.errorText = "Wrong Username or Password!";
        }
        
      }
      else {
        console.log("Failed to login")
        this.errorText = "Wrong Username or Password!";
      }
    });
    this.router.navigateByUrl('/home');
  }

  login() { 
    this.authService.login(this.model).subscribe(next => {
      console.log('Logged in successfully');
    }, error => {
      console.log('Failed to login');
      this.errorText = "Wrong Username or Password!";
    });
    this.router.navigateByUrl('/home');
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.clear();
  }

  loggedIn() {
    const token = localStorage.getItem('token');
    return !token;
  }

  private delay(ms: number) {
    return new Promise(resolve => setTimeout(resolve, ms));
  }

}
