export class User {
  id: number;
  fullName: string;
  userName: string;
  password: string;
  email: string;
  rolId: number;
  passwordTemp: string;
  changePassword: boolean;

  constructor(id: number, fullname: string, userName: string, email: string, rolId: number ) {
    this.id       = id;
    this.fullName = fullname;
    this.userName = userName;
    this.email    = email;
    this.rolId    = rolId;
  }

}
