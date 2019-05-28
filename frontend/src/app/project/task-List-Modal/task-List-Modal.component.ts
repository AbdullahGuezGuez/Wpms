import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';

@Component({
  selector: 'app-task-List-Modal',
  templateUrl: './task-List-Modal.component.html',
  styleUrls: ['./task-List-Modal.component.css']
})
export class TaskListModalComponent implements OnInit {

  constructor(public dialogRef: MatDialogRef<TaskListModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) { 

    }

  ngOnInit() {
  }

  close() {
    this.dialogRef.close();
  }
}
