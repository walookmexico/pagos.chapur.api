import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { DialogsAlertsComponent } from '../dialogs-alerts/dialogs-alerts.component';


@Component({
  selector: 'app-dialogs-table',
  templateUrl: './dialogs-table.component.html',
  styleUrls: ['./dialogs-table.component.css']
})
export class DialogsTableComponent implements OnInit {

  constructor(public dialogRef: MatDialogRef<DialogsAlertsComponent>,
    @Inject(MAT_DIALOG_DATA) public data: string) { }

  ngOnInit() {
  }

  onClose() {
    this.dialogRef.close(true);
  }

}
