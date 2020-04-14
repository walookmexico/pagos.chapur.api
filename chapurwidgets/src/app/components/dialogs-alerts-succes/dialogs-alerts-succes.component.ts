import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef } from '@angular/material';
import { MAT_DIALOG_DATA } from '@angular/material';

@Component({
  selector: 'app-dialogs-alerts-succes',
  templateUrl: './dialogs-alerts-succes.component.html',
  styleUrls: ['./dialogs-alerts-succes.component.css']
})
export class DialogsAlertsSuccesComponent implements OnInit {

  constructor(public dialogRef: MatDialogRef<DialogsAlertsSuccesComponent>,
    @Inject(MAT_DIALOG_DATA) public data: string) { }

  ngOnInit() {
  }

}
