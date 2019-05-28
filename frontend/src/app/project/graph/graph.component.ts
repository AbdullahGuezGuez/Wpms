import { Component, OnInit, Input, SimpleChanges } from '@angular/core';
import { ProjectService } from 'src/app/_services/project.service';
import { ChartDataSets, ChartOptions } from 'chart.js';
import { Color, BaseChartDirective, Label } from 'ng2-charts';

@Component({
  selector: 'app-graph',
  templateUrl: './graph.component.html',
  styleUrls: ['./graph.component.css']
})
export class GraphComponent implements OnInit {
  @Input() projectId: any;
  project: any;
  model: any = {};
  toggleGraph: boolean = false;
  public pieChartLabels = ['In Backlog', 'In Production', 'In Test', 'Done'];
  public pieChartType = 'pie';
  public pieChartColors:{}[]= [ { backgroundColor: ['#F3A09E', '#6F78F2', '#ffe066', '#3ABAAF'] } ];

  selectedBoard: any;
  availableBoards: any;


  constructor( private _projectService: ProjectService) { }

  ngOnInit() {
    this.getProject()
    this.getAvailableBoards()
  }

  ngOnChanges(changes: SimpleChanges) {
    changes.projectId;
    this.getProject()
    this.getAvailableBoards()
  }

  getProject(){
    this._projectService.getProjectValues(this.projectId).subscribe(data => this.project = data);
  }

  getAvailableBoards(){
    this._projectService.getAvaliableBoards().subscribe(data => this.availableBoards = data);
  }

  async changeBoardConnection(){
    this.model.projectId = this.projectId;
    this.model.trelloBoardId = this.selectedBoard;
    this._projectService.changeTrelloBoardConnection(this.model).subscribe();
    await this.delay(800);
    document.getElementById("closeModal").click();
    this.getProject();
    this.getAvailableBoards();
  }

  async removeBoardConnection(){
    this.model.projectId = this.projectId;
    this._projectService.removeTrelloBoardConnection(this.model).subscribe();
    await this.delay(800);
    this.getProject();
    this.getAvailableBoards();
  }

  async createNewTrelloBoard(){
    this.model.projectId = this.projectId;
    this._projectService.createTrelloBoard(this.model).subscribe();
    await this.delay(500);
    document.getElementById("closeModal").click();
    this.getProject();
    this.getAvailableBoards();
  }

  toggleGraphs()
  {
    this.toggleGraph = !this.toggleGraph;
  }

  private delay(ms: number)
    {
      return new Promise(resolve => setTimeout(resolve, ms));
    }

//_____________________________________________________________________________________________________ NEDAN ÄR FÖR BURNDOWN SOM ÄR UTKOMMENTERAD I HTML

  private generateNumber(i: number) {
    return Math.floor((Math.random() * (i < 2 ? 100 : 1000)) + 1);
  }

  // events
  public chartClicked({ event, active }: { event: MouseEvent, active: {}[] }): void {
    console.log(event, active);
  }

  public chartHovered({ event, active }: { event: MouseEvent, active: {}[] }): void {
    console.log(event, active);
  }

  public pushOne() {
    this.lineChartData.forEach((x, i) => {
      const num = this.generateNumber(i);
      const data: number[] = x.data as number[];
      data.push(num);
    });
    this.lineChartLabels.push(`Label ${this.lineChartLabels.length}`);
  }

  public changeColor() {
    this.lineChartColors[0].borderColor = 'green';
    this.lineChartColors[0].backgroundColor = `rgba(0, 255, 0, 0.3)`;
  }


public lineChartData: ChartDataSets[] = [
  { data: [349, 290, 230, 189, 133, 78, 38], label: 'Estimated' },
  { data: [349, 296, 229, 190, 124, 67, 32], label: 'Actually' }
];
public lineChartLabels: Label[] = ['Sprint 1', 'Sprint 2', 'Sprint 3', 'Sprint 4', 'Sprint 5', 'Sprint 6', 'Sprint 7', 'Sprint 8'];
public lineChartOptions: (ChartOptions & { annotation: any }) = {
  responsive: true,
  scales: {
    // We use this empty structure as a placeholder for dynamic theming.
    xAxes: [{}],
    yAxes: [
      {
        id: 'y-axis-0',
        position: 'left',
      }
    ]
  },
  annotation: {
    annotations: [
      {
        type: 'line',
        mode: 'vertical',
        scaleID: 'x-axis-0',
        value: 'March',
        borderColor: 'orange',
        borderWidth: 2,
        label: {
          enabled: true,
          fontColor: 'orange',
          content: 'LineAnno'
        }
      },
    ],
  },
};
public lineChartColors: Color[] = [
  { // grey'', ''rgba(148,159,177,1)
    backgroundColor: 'rgba(243,159,158,0.1)',
    borderColor: '#F07F7B',
    pointBackgroundColor: 'rgba(148,159,177,1)',
    pointBorderColor: '#fff',
    pointHoverBackgroundColor: '#fff',
    pointHoverBorderColor: 'rgba(148,159,177,0.8)'
  },
  { // dark greyrgba(77,83,96,1)
    backgroundColor: 'rgba(58,183,176,0.1)',
    borderColor: '#3ABAAF',
    pointBackgroundColor: 'rgba(77,83,96,1)',
    pointBorderColor: '#fff',
    pointHoverBackgroundColor: '#fff',
    pointHoverBorderColor: 'rgba(77,83,96,1)'
  },
  { // red
    backgroundColor: 'rgba(255,0,0,0.3)',
    borderColor: 'red',
    pointBackgroundColor: 'rgba(148,159,177,1)',
    pointBorderColor: '#fff',
    pointHoverBackgroundColor: '#fff',
    pointHoverBorderColor: 'rgba(148,159,177,0.8)'
    
  }
];
public lineChartLegend = true;
public lineChartType = 'line';

//_____________________________________________________________________________________________________


}
