import { Component, OnInit } from '@angular/core';
import { ProjectService } from 'src/app/_services/project.service';
import { Color, BaseChartDirective, Label } from 'ng2-charts';

@Component({
  selector: 'app-project-dashboard',
  templateUrl: './project-dashboard.component.html',
  styleUrls: ['./project-dashboard.component.css']
})
export class ProjectDashboardComponent implements OnInit {
  public pieChartLabels = ['In Backlog', 'In Production', 'In Test', 'Done'];
  public pieChartLabelsWithoutTrello = ['Not enough Trellodata'];
  public pieChartType = 'pie';
  public pieChartColors:{}[]= [ { backgroundColor: ['#F3A09E', '#6F78F2', '#ffe066', '#3ABAAF'] } ];
  public pieChartColorsWithoutTrello:{}[]= [ { backgroundColor: ['#404040'] } ];
  data: any;

  constructor( private _projectService: ProjectService) { }

  ngOnInit() {
    this.getDashBoardProjects();
  }

  getDashBoardProjects()
  {
    this._projectService.getDashboardProjects().subscribe(res => this.data = res);
  }

  goToTrelloCard(url: any) {
    if(url == null || url == "")
    {
      console.log("No Trelloboard for this project");
    }
    else
      window.open(url);
  }

}
