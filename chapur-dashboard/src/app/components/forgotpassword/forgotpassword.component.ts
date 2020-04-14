import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators, FormArray } from '@angular/forms';
import { UsersService } from '../../services/users.service';
import { Router, ActivatedRoute } from '@angular/router';
import { MatDialog } from '@angular/material';

// Compnente de los alert
import { DialogsAlertsComponent } from '../dialogs-alerts/dialogs-alerts.component';

@Component({
  selector: 'app-forgotpassword',
  templateUrl: './forgotpassword.component.html',
  styleUrls: ['./forgotpassword.component.css']
})
export class ForgotpasswordComponent implements OnInit {

  frmRecovery: FormGroup;
  working: boolean;
  showMessageBar: boolean;
  typeMessageBar: string;
  titleMessageBar: string;
  descripcionMessageBar: string;


  constructor(private userService: UsersService,
              private router: Router,
              public dialog: MatDialog) {

    this.showMessageBar  = false;
    this.typeMessageBar  = 'yellow';
    this.titleMessageBar = 'Error de autenticacón!';
    this.descripcionMessageBar = 'descripcionMessageBar';

    this.frmRecovery = new FormGroup({
      'email': new FormControl('', [ Validators.required,
                                     Validators.pattern('[a-z0-9._%+-]+@[a-z0-9.-]+.[a-z]{2,3}$')])
    });

  }

  ngOnInit() {
  }

  onResetPassword(): void {
    this.working = true;
    console.log(this.frmRecovery.value.email);
    this.userService.forgotPassword(this.frmRecovery.value.email).subscribe(response => {
      console.log(response);
      if (response['isSuccess']) {
        this.working = false;
          this.showSuccessMessageBar(response.messages);
          setTimeout(() => {
            this.router.navigateByUrl('login');
          }, 5000);
      } else {
          this.working = false;
          this.showErrorMessageBar(response.messages);
      }
    });
  }

  showErrorMessageBar(message: string): void {
    this.titleMessageBar = 'Ocurrio un problemas:';
    this.descripcionMessageBar = message;
    this.typeMessageBar = 'yellow';
    this.showMessageBar = true;
  }

  showSuccessMessageBar(message: string): void {
    this.titleMessageBar = 'Solicitud exitosa:';
    this.descripcionMessageBar = message;
    this.typeMessageBar = 'blue';
    this.showMessageBar = true;
  }

}
