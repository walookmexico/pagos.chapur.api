import { Component, OnInit, ViewChild, ViewChildren } from '@angular/core';
import { StorageService } from '../../services/storage.service';
import { UsersService } from '../../services/users.service';
import { ChangepassworddialogComponent } from '../changepassworddialog/changepassworddialog.component';
import { DialogsAlertsComponent } from '../dialogs-alerts/dialogs-alerts.component';
import { MatDialog } from '@angular/material';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit {

  showUserInfo: boolean;
  showLoginInfo: boolean;
  statusPassword: boolean;
  rol: string;
  name: string;
  pass: any;
  res: any;
  working: boolean;

  constructor(private storageService: StorageService,
              public dialog: MatDialog,
              private usersService: UsersService) {

    this.showUserInfo = this.storageService.loggedIn();

    if ( this.showUserInfo ) {
      this.rol  = this.storageService.rol;
      this.name = this.storageService.name;

      if (this.storageService.changePassword) {
        this.changesPassword();
      }
    }
  }

  ngOnInit() {
    this.showUserInfo = this.storageService.loggedIn();
  }

  signOut() {
    this.storageService.logoutRedirect();
  }

  // Muestra venta confirmar
  changesPassword(): void {
      const dialogRef = this.dialog.open(ChangepassworddialogComponent, {
        width: '390px',
        height: '360px',
        disableClose : true,
        data: { }
      });

      dialogRef.afterClosed().subscribe(response => {
          // console.log(`Dialog colsed: ${response.res}`);
          this.working = true;
          if (response.res) {
            this.usersService.updatePassword(response.data.passwordActual, response.data.password ).subscribe(res => {
              // console.log(res);
              if (res.isSuccess) {
                this.storageService.changePassword = false;
                this.openAlert(res.messages, 'Success');
                this.working = false;
              } else {

                // this.changesPassword();
                this.openAlert(res.messages, 'Warning');
                this.working = false;
              }
            });
          } else {
            this.storageService.changePassword = false;
            this.working = false;
          }

      });
  }

  // Muestra vetana alert
  openAlert(message: string, title: string): void {
    const dialogRef = this.dialog.open(DialogsAlertsComponent, {
      width: '400px',
      disableClose : true,
      data: { message: message, title: title }
    });

    dialogRef.afterClosed().subscribe(response => {

    });
  }


}
