import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { ProjectsComponent } from './project/projects/projects.component';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './admin/register/register.component';
import { RoleManagementComponent } from './admin/role-management/role-management.component';
import { OrganizationManagementComponent } from './admin/organization-management/organization-management.component';
import { ProjectComponent }  from './project/project/project.component';
import { CustomerComponent } from './customer/customer.component';
import { CustomerListComponent } from './customer-list/customer-list.component';
import { CreateActivityComponent } from './activity/createActivity/createActivity.component';
import { UserManagementComponent } from './admin/user-management/user-management.component';
import { CreatenextstepComponent } from './activity/createnextstep/createnextstep.component';
import { AllActivitiesComponent } from './activity/all-activities/all-activities.component';
import { TodosComponent } from './activity/todos/todos.component';


const routes: Routes = [
  { path: 'home', component: HomeComponent },
  { path: 'users', component: UserManagementComponent },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'rolemanagement', component: RoleManagementComponent},
  { path: "organizationmanagement", component: OrganizationManagementComponent },
  { path: 'project/:id', component: ProjectComponent},
  { path: 'projects', component: ProjectsComponent},
  { path: 'customer/:id', component: CustomerComponent},
  { path: 'customerlist', component: CustomerListComponent},
  { path: 'createactivity', component: CreateActivityComponent},
  { path: 'createnextstep/:actId/:custId', component: CreatenextstepComponent},
  { path: 'allactivities', component: AllActivitiesComponent},
  { path: 'todos', component: TodosComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
