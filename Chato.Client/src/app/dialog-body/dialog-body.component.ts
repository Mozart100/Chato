import { Component } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { Observable, tap } from 'rxjs';
import { LoginService } from '../service/login-service.service';
import AppConsts from '../Consts/AppConsts';
import { DialogRef } from '@angular/cdk/dialog';



@Component({
  selector: 'app-dialog-body',
  templateUrl: './dialog-body.component.html',
  styleUrls: ['./dialog-body.component.scss']
})
export class DialogBodyComponent {

  form = this.fb.group({
    userName: ['', [Validators.required, Validators.minLength(4)]],
    userPassword: ['', [Validators.required, Validators.minLength(4)]],
  });

  constructor(private fb: FormBuilder, private _loginService:LoginService , private _dialog:DialogRef<DialogBodyComponent>) {
  }


  onSubmit() {
    console.log('submmited',this.form);
    debugger;


    this._loginService.resistrationUser(this.form.value.userName!, this.form.value.userPassword!).pipe(
      tap(response => console.log(response))
    ).subscribe(
      response => {
        // Handle the successful response here
        console.log('User registered successfully:', response);
        sessionStorage.setItem(AppConsts.Token, response.token!);
        this._dialog.close();
      },
      error => {
        // Handle the error response here
        console.error('Registration error:', error);
      }
    );
  }


}
