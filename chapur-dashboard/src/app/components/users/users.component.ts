import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormControl, Validators, FormArray, NgForm } from '@angular/forms';
import { MatPaginator, MatTableDataSource } from '@angular/material';
import { UsersService } from '../../services/users.service';
import { User } from '../../modules/user';
import { MatDialog } from '@angular/material';
// Compnente de los alert
import { DialogsComponent } from '../dialogs/dialogs.component';
import { DialogsAlertsComponent } from '../dialogs-alerts/dialogs-alerts.component';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.css']
})
export class UsersComponent implements OnInit {
  working: boolean;
  frmUser: FormGroup;
  // Bandera para mostrar ocular campos de password
  closePassword: boolean;
  editStatus: boolean;
  // Variable almacena opcion seleccionada en el alert
  dialogResult: any ;
  displayedColumns: string[];
  dataSource: User[];
  roles: any;

  @ViewChild('form') myForm;

  constructor(private userService: UsersService, public dialog: MatDialog) {
    // INICIALIZAR DATOS
    this.working       = false;
    this.closePassword = true;
    this.editStatus    = false;
    // CARGAR USUARIOS REGISTRADOS
    this.loadUsers();
    this.initForm();

    this.displayedColumns = ['id', 'fullName', 'userName', 'edit'];
    this.roles = [
      { value: 1, viewValue: 'Administrador' },
      { value: 2, viewValue: 'Consulta' }
    ];
  }

  /*--------------------  EVENTS -------------------------------*/

  // Inicializar componente
  ngOnInit() {}

  // Eliminar
  onDelete(user: User): void {
    // console.log(user['id']);
    this.openDialog('¿Eliminar el registro?', 'Confirmación', true, user);
  }
  // Busca y muestra los datos del usurio para editarlos
  onEdit(user: User): void {
    this.closePassword = false;
    this.editStatus = true;

    this.working = true;
    this.userService.getUserUserId(user['id']).subscribe(response => {
      this.initFormEdit(user);
      this.working = false;
    }, err => {
      this.working = false;
    });
  }

  // Cancelar
  onCancel(event: any): void {
    event.preventDefault();
    this.closePassword = true;
    this.resetForm();
  }

  // Muestra venta confirmar
  openDialog(message: string, title: string, showConfirm: boolean, user: User): void {
    const dialogRef = this.dialog.open(DialogsComponent, {
      width: '300px',
      data: { message: message, title: title, showConfirm: showConfirm }
    });

    dialogRef.afterClosed().subscribe(response => {
      // console.log(`Dialog colsed: ${response}`);
      this.dialogResult = response;


      if (response) {

        this.working = true;

        this.userService.deleteUser(user['id']).subscribe(
          res => {
            if (res.isSuccess) {
              this.loadUsers();
              this.openAlert(res.messages, 'Success');
            }
            this.working = false;
          },
          err => {
            if (err.status === 0) {
              this.openAlert(
                'Problemas de conexión con el servidor',
                'Warning'
              );
            } else if (err.status === 504) {
              this.openAlert('Error 504, favor de intentar más tarde', 'Warning');
            } else {
              this.openAlert('Error desconocido', 'Warning');
            }
            this.working = false;
          });
      }
    });
  }

  // Muestra vetana alert
  openAlert(message: string, title: string): void {
    const dialogRef = this.dialog.open(DialogsAlertsComponent, {
      width: '400px',
      data: { message: message, title: title }
    });

    dialogRef.afterClosed().subscribe(response => {
      console.log(`Dialog colsed: ${response}`);
      this.dialogResult = response;
    });
  }

  // Evento submit para el formulario
  onSubmit(): void {

    this.working = true;
    let userData: User;

    userData = new User( this.editStatus ? this.frmUser.value.userId : 0,
         this.frmUser.value.fullName,
         this.frmUser.value.userName,
         this.frmUser.value.email,
         this.frmUser.value.rolId );

    if (this.editStatus) {
      this.updateUser(userData);
    } else {
      userData.password = this.frmUser.value.password;
      this.saveNewUser(userData);
    }
  }

  /*--------------------  FUNCTIONS  -------------------------------*/

  // CARGAR USUARIOS REGISTRADOS
  loadUsers(): void {
    this.working = true;
    let users: User[] = [];
    this.userService.getAllUser().subscribe(
      result => {
        if (result.isSuccess) {
          for (let user of result.data) {
            users.push( new User( user.id, user.fullName, user.userName, user.email, user.rolId ) );
          }
        }
        this.dataSource = users;
        this.working = false;
      },
      err => {
        if (err.status === 0) {
          this.openAlert('Problemas de conexión con el servidor', 'Warning');
        } else {
          this.openAlert('Error desconcido', 'Warning');
        }
        this.working = false;
      }
    );
  }

  // Guardar los datos de un nuevo usuario
  saveNewUser(user: User): void {

    this.userService.postUser(user).subscribe(
      response => {
        // console.log(response);
        if (response.isSuccess) {
          // RE-CARGA USUARIOS REGISTRADOS DESPUES DEL INSERTAR
          this.openAlert(response.messages, 'Success');
          this.loadUsers();
          this.resetForm();
          this.working = false;


        }
      },
      err => {
        if (err.status === 0) {
          this.openAlert(
            'Problemas de conexión con el servidor',
            'Notificación'
          );
        } else {
          this.openAlert('Error desconcido', 'Notificación');
        }
        this.working = false;
      }
    );

  }

  // Actualizar los datos de un usuario existente
  updateUser(user: User): void {

     // funcion para enviar a guardar los datos
     this.working = true;
     this.userService.updateUser(user).subscribe(
       response => {
        if (response.isSuccess) {
          this.closePassword = true;
          this.openAlert(response.messages, 'Success');
          this.loadUsers();
          this.resetForm();
          this.initForm();
        } else {
          this.openAlert(response.messages, 'warning');
        }
        this.working = false;

        // console.log(response);
      }, err => {
      if (err.status === 0) {
        this.openAlert(
          'Problemas de conexión con el servidor',
          'Notificación'
        );
      } else {
        this.openAlert('Error desconcido', 'Notificación');
      }
      this.working = false;
    });
  }

  // Inicializa el formulario
  initForm(): void {
    this.frmUser = new FormGroup({
      userId: new FormControl(null),
      fullName: new FormControl('', [
        Validators.required,
        Validators.maxLength(30)
      ]),
      userName: new FormControl('', [Validators.required]),
      password: new FormControl('', [Validators.required]),
      confirmPassword: new FormControl(''),
      email: new FormControl('', [
        Validators.required,
        Validators.pattern('[a-z0-9._%+-]+@[a-z0-9.-]+.[a-z]{2,3}$')
      ]),
      rolId: new FormControl('', [Validators.required])
    });

    this.frmUser.controls['confirmPassword'].setValidators([
      Validators.required,
      this.equalsPassword.bind(this.frmUser)
    ]);


  }

  // Inicializar el formoulario para edición
  initFormEdit(user: User): void {

    /* INIT FORM */
    this.frmUser = new FormGroup({
      userId: new FormControl(user['id']),
      fullName: new FormControl(user['fullName'], [
        Validators.required,
        Validators.maxLength(30)
      ]),
      userName: new FormControl(user['userName'], [Validators.required]),
      email: new FormControl(user['email'], [
        Validators.required,
        Validators.pattern('[a-z0-9._%+-]+@[a-z0-9.-]+.[a-z]{2,3}$')
      ]),
      rolId: new FormControl(user['rolId'], [Validators.required])
    });
  }

  // Inicializar el formulario
  resetForm(): void {
    this.frmUser.reset();
    this.myForm.resetForm();
    this.frmUser.markAsUntouched();
    this.frmUser.markAsPristine();
    this.frmUser.updateValueAndValidity();
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
