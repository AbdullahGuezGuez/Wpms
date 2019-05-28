import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { Router } from '@angular/router';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HttpClientModule } from '@angular/common/http';
import { LoginComponent } from './login/login.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AuthService } from './_services/auth.service';
import { HomeComponent } from './home/home.component';
import { ProjectsComponent } from './project/projects/projects.component';
import { RegisterComponent } from './admin/register/register.component';
import { AdminService } from './_services/admin.service';
import { RolesModalComponent } from './admin/roles-modal/roles-modal.component';
import { UserManagementComponent } from './admin/user-management/user-management.component';
import { ModalModule } from 'ngx-bootstrap';
import { RoleManagementComponent } from './admin/role-management/role-management.component';
import { TabsModule } from 'ngx-tabset';
import { OrganizationManagementComponent } from './admin/organization-management/organization-management.component';
import { CheckPasswordDirectiveService } from 'src/app/_services/CheckPasswordDirective.service';
import { JwtModule } from '@auth0/angular-jwt';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { ChooseorgComponent } from './login/chooseorg/chooseorg.component';
import { SweetAlert2Module } from '@sweetalert2/ngx-sweetalert2';
import { MatCardModule, MatButtonModule, MatSelectModule,MatDialogModule, MatRadioButton} from '@angular/material';
import { ProjectComponent } from './project/project/project.component';
import { DatepickerModule, BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { MatRadioModule, MatProgressSpinnerModule, MatTableModule, MatPaginatorModule, MatSortModule, MatFormFieldModule, MatInputModule, MatOptionModule, MatCheckboxModule, MatChipsModule } from '@angular/material';
import { FlexLayoutModule } from '@angular/flex-layout';
import { CustomerComponent } from './customer/customer.component';
import { CustomerListComponent } from './customer-list/customer-list.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { WavesModule } from 'angular-bootstrap-md';
import { ChartsModule } from 'ng2-charts';
import { ActivityComponent } from './activity/activity.component';
import { CreateActivityComponent } from './activity/createActivity/createActivity.component';
import { MatListModule } from '@angular/material/list';
import { MatDatepickerModule, MatNativeDateModule } from '@angular/material';
import { AmazingTimePickerModule } from 'amazing-time-picker';
import { GraphComponent } from './project/graph/graph.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { CreatenextstepComponent } from './activity/createnextstep/createnextstep.component';
import { TaskListModalComponent } from './project/task-List-Modal/task-List-Modal.component';
import { ChangePasswordComponent } from './admin/change-password/change-password.component';
import { AllActivitiesComponent } from './activity/all-activities/all-activities.component';
import { TodosComponent } from './activity/todos/todos.component';
import { ProjectDashboardComponent } from './project/project-dashboard/project-dashboard.component';

export function tokenGetter() {
   return localStorage.getItem('token');
 }

@NgModule({
   declarations: [
      ProjectsComponent,
      HomeComponent,
      AppComponent,
      LoginComponent,
      ProjectsComponent,
      RegisterComponent,
      UserManagementComponent,
      RolesModalComponent,
      CheckPasswordDirectiveService,
      RoleManagementComponent,
      OrganizationManagementComponent,
      ChooseorgComponent,
      ProjectComponent,
      CustomerComponent,
      CustomerListComponent,
      ActivityComponent,
      CreateActivityComponent,
      GraphComponent,
      CreatenextstepComponent,
      TaskListModalComponent,
      ChangePasswordComponent,
      AllActivitiesComponent,
      TodosComponent,
      ProjectDashboardComponent
   ],
   imports: [
      BrowserModule,
      AppRoutingModule,
      HttpClientModule,
      MatRadioModule,
      BrowserAnimationsModule,
      MatOptionModule,
      FormsModule,
      MatInputModule,
      MatDialogModule,
      MatFormFieldModule,
      ReactiveFormsModule,
      MatCardModule,
      MatSelectModule, 
      MatButtonModule,
      ModalModule.forRoot(),
      TabsModule.forRoot(),
      DragDropModule,
      SweetAlert2Module.forRoot({
         buttonsStyling: false,
         customClass: 'modal-content',
         confirmButtonClass: 'btn btn-primary',
         cancelButtonClass: 'btn'
     }),
      JwtModule.forRoot({
         config: {
           tokenGetter: tokenGetter,
           whitelistedDomains: ['localhost:5000'],
           blacklistedRoutes: ['localhost:5000/api/auth']
         }
       }),
       BsDatepickerModule.forRoot(),
       DatepickerModule.forRoot() ,
      MatTableModule,
      MatPaginatorModule,
      MatSortModule,
      BrowserAnimationsModule,
      MatFormFieldModule,
      MatInputModule,
      FlexLayoutModule,
      ChartsModule,
      WavesModule,
      MatListModule,
      MatDatepickerModule,
      MatNativeDateModule,
      AmazingTimePickerModule,
      MatCheckboxModule,
      MatChipsModule,
      NgbModule,
      MatProgressSpinnerModule
   ],
   providers: [
      AuthService,
      AdminService,
   ],
   entryComponents: [
      RolesModalComponent,
      TaskListModalComponent
   ],
   bootstrap: [
      AppComponent
   ],
   schemas: [
      CUSTOM_ELEMENTS_SCHEMA,
   ]
})
export class AppModule { }
