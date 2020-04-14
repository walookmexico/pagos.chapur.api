import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef } from '@angular/material';
import { MAT_DIALOG_DATA } from '@angular/material';

@Component({
  selector: 'app-dialogs-alerts',
  templateUrl: './dialogs-alerts.component.html',
  styleUrls: ['./dialogs-alerts.component.css']
})
export class DialogsAlertsComponent implements OnInit {

  constructor(public dialogRef: MatDialogRef<DialogsAlertsComponent>,
              @Inject(MAT_DIALOG_DATA) public data: string) { }

  ngOnInit() {
  }

  onCloseConfirm() {
    this.dialogRef.close(true);
  }

}
