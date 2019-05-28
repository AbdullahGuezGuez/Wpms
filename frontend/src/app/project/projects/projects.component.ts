import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { Router } from '@angular/router';
import { ProjectService } from '../../_services/project.service';
import { AdminService } from '../../_services/admin.service';
import { iProject } from '../../_models/iProject';
import { map } from "rxjs/operators";
import { MatPaginator, MatTableDataSource, MatSort, MatSortModule, MatFormFieldModule, MatInputModule } from '@angular/material';
import { CustomerService } from 'src/app/_services/customer.service';
import { AuthService } from 'src/app/_services/auth.service';

@Component({
  selector: 'app-projects',
  templateUrl: './projects.component.html',
  styleUrls: ['./projects.component.css']
})
export class ProjectsComponent implements OnInit, AfterViewInit {
  displayedColumns: string[] = ["priority"];
  dataSource = new MatTableDataSource<iProject>();
  noData = this.dataSource.connect().pipe(map(data => data.length === 0));
  includeTrello = true;
  customers: any;
  contacts: any;
  project: any = {};
  users: any;
  waiter: boolean = false;
  showActive: boolean = true;

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  constructor(public _authService: AuthService, private _customerService: CustomerService, private projectService: ProjectService,
    private adminService: AdminService, private _router: Router) { }

  ngOnInit() {
    this.getAllProjects();
    this.getUsers();
    this.getAllCustomers();
  }
  
  ngAfterViewInit() {
    this.dataSource.sort = this.sort;
    this.dataSource.paginator = this.paginator;
  }

  public doFilter = (value: string) => {
    this.dataSource.filter = value.trim().toLocaleLowerCase();
  };
  
  getAllProjects() {
    this.adminService.getProjectList().subscribe(res => {
      this.dataSource.data = res as iProject[];
    });
  }

  getAllUnarchivedProjects() {
    this.adminService.getProjectList().subscribe(res => {
      this.dataSource.data = res as iProject[];
    });
    this.loader();
  }

  getAllArchivedProjects() {
    this.adminService.getUnactiveProjectList().subscribe(res => {
      this.dataSource.data = res as iProject[];
    });
    this.loader();
  }

  async loader()
  {
    this.waiter = true;
    await this.delay(1000);
    this.showActive = !this.showActive;
    this.waiter = false;
  }

  selectedProject(projectId: any) {
    this._router.navigateByUrl("project/" + projectId);
  }

  goToTrelloBoard(url: any) {
    if(url == null || url == "")
    {
      console.log("No Trelloboard for this project");
    }
    else
      window.open(url);
  }

  goToToggl(url: any){
    if(url == null || url == "")
    {
      console.log("No Toggl for this project");
    }
    else
      window.open(url);
  }

  //___________________________________________________________________________________________________________

  async createProject() {
    this.project.includeTrello = !this.includeTrello;
    this.projectService.createProject(this.project).subscribe(res => {
      document.getElementById("closeProjectModal").click();
      this._router.navigateByUrl("project/" + (res));
    });
  }

  toggleTrello() {
    this.includeTrello = !this.includeTrello;
    return;
  }

  getUsers() {
    this.adminService.getAllUsers().subscribe(data => this.users = data);
  }

  getAllCustomers() {
    this._customerService.getAllCustomersForOrganization().subscribe(data => this.customers = data);
  }

  getAllContactsForSelectedCustomer(customerId) {
    this._customerService.getContactpersonsByCustomerId(customerId).subscribe(data => this.contacts = data);
  }

  private delay(ms: number)
    {
      return new Promise(resolve => setTimeout(resolve, ms));
    }

  /*   SelectedProject(iProject) {
      console.log(iProject);
      for (let i = 0; i < this.dataSource.data.length; i++) {
        if (this.dataSource.data[i].id== iProject) {
          console.log(this.dataSource.data[i].priority);
          console.log(this.dataSource.data[i].endDate);
          console.log(this.dataSource.data[i].budget);
          console.log(this.dataSource.data[i].id);
          //! ROUTA MED ID
          this._router.navigateByUrl("project/" + this.dataSource.data[i].id);
        }
      }
    }
  
   public postBoard() {
      const boardName = (document.getElementById('boardName') as HTMLInputElement).value;
      const choosenType = (document.getElementById('choosenType1') as HTMLInputElement).value;
  
      const uri = 'https://api.trello.com/1/boards/?name=' + boardName + '&defaultLabels=true&defaultLists=true&keepFromSource=' + choosenType + 
      '&prefs_permissionLevel=private&prefs_voting=disabled&prefs_comments=members&prefs_invitations=members&prefs_selfJoin=true&prefs_cardCovers=true&prefs_background=blue&prefs_cardAging=regular&key=9dc5e140e89bd1ff02b261a4ebeaf519&token=6469b110f8d594da9783fed645a2a3753ee6920dcfe0c40ef38e61bdaf8454c1';
      this._boardService.postTrelloBoard(uri).subscribe(next => {
        console.log('Post succeded');
      });
  
    }
  
    public archiveBoard() {
      const choosenBoardId = ((document.getElementById('boardToArchive') as HTMLInputElement).value);
      const uri = 'https://api.trello.com/1/boards/' + choosenBoardId +
       '?closed=true&key=9dc5e140e89bd1ff02b261a4ebeaf519&token=6469b110f8d594da9783fed645a2a3753ee6920dcfe0c40ef38e61bdaf8454c1';
      this._boardService.archiveTrelloBoard(uri).subscribe(
        () => console.log('Board has been archived')
      );
    }*/


  // ! Används inte
  /* getValues() {
       const getUri = 'https://api.trello.com/1/members/me/boards?key=8647cda40947c5f59daaa1c3f5173a1a&token=5e73a3d20653d1e9f97812fa1572a61499b84ffd6954f1b33f4f93d69fd0fdff';
 
       this.http.get(getUri).subscribe(response => {
         this.values = response;
       }, error => {
         console.log(error);
       });
 
     }*/



}
