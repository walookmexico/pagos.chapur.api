import { Component, OnInit, Inject, AfterViewInit, ViewChild, ElementRef, Renderer  } from '@angular/core';
import { MatDialogRef } from '@angular/material';
import { MAT_DIALOG_DATA } from '@angular/material';
import { FormGroup, FormControl, Validators, FormArray } from '@angular/forms';

@Component({
  selector: 'app-changepassworddialog',
  templateUrl: './changepassworddialog.component.html',
  styleUrls: ['./changepassworddialog.component.css']
})
export class ChangepassworddialogComponent implements OnInit, AfterViewInit  {

  frmPassword: FormGroup;
  @ViewChild('btnRememberLater') btnRememberLater: ElementRef;

  constructor(private renderer: Renderer,
    public dialogRef: MatDialogRef<ChangepassworddialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: string) {

      this.frmPassword = new FormGroup({
        passwordActual: new FormControl('', [Validators.required]),
        password: new FormControl('', [Validators.required, Validators.minLength(8)]),
        confirmPassword: new FormControl(''),
      });

      this.frmPassword.controls['confirmPassword'].setValidators([
        Validators.required,
        this.equalsPassword.bind(this.frmPassword)
      ]);

     }

  ngOnInit() {}

  ngAfterViewInit(): void {
    // this.btnRememberLater.nativeElement.focus();
    // this.renderer.invokeElementMethod(this.btnRememberLater.nativeElement, 'focus');
  }

  onCloseChange() {
    let data = {
      'res' : true,
      'data': this.frmPassword.value
    };
    this.dialogRef.close(data);
  }

  onCloseLater() {
    let data = {
      'res' : false,
    };
    this.dialogRef.close(data);
  }

  /*--------------------  VALIDATORS -------------------------------*/
  equalsPassword(control: FormControl): { [s: string]: boolean } {
    // console.log(this);
    let form: any = this;

    if (control.value !== form.controls['password'].value) {
      return {
        equalspassword: true
      };
    }
    return null;
  }
}
