import { Component, OnInit, Inject, AfterViewInit, ViewChild, Renderer } from '@angular/core';
import { FormGroup, FormControl, Validators, FormArray } from '@angular/forms';
import { MatDialogRef } from '@angular/material';
import { MAT_DIALOG_DATA } from '@angular/material';
import { TokenService } from '../../services/token.service';

@Component({
  selector: 'app-dialogs-token',
  templateUrl: './dialogs-token.component.html',
  styleUrls: ['./dialogs-token.component.css']
})
export class DialogsTokenComponent implements OnInit {
  frmToken: FormGroup;

  constructor(private renderer: Renderer,
              public dialogRef: MatDialogRef<DialogsTokenComponent>,
              @Inject(MAT_DIALOG_DATA) public data: string,  private tokenService: TokenService) {

    this.frmToken = new FormGroup({
      token: new FormControl('', [Validators.required, Validators.minLength(6), Validators.maxLength(6)])
    });
  }

  ngOnInit() {
  }

  onCloseConfirm() {
    const data = {
      'status': true,
      'token': this.frmToken.value
    };
    this.dialogRef.close(data);
  }

  onCloseCancel() {
    const data = {
      'status': false
    };
    this.dialogRef.close(data);
  }

  getToken() {
    this.tokenService.getToken(
      this.data['numberTarget'],
      this.data['idStore'],
      this.data['email'])
    .subscribe(response => console.log(response));
  }

}
