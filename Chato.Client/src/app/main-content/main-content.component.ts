import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { Observable, tap } from 'rxjs';
import { LoginService } from '../service/login-service.service';

@Component({
  selector: 'app-main-content',
  templateUrl: './main-content.component.html',
  styleUrls: ['./main-content.component.scss'],
})
export class MainContentComponent implements OnInit {
  
  form = this.fb.group({
    userName: ['', [Validators.required, Validators.minLength(4)]],
    userPassword: ['', [Validators.required, Validators.minLength(4)]],
  });

  constructor(private fb: FormBuilder, private _loginService:LoginService) {
    // const tmp =fb.group()
  }

  ngOnInit(): void {}

  get getCurseTitle() {
    return this.form.controls['userName'];
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
      },
      error => {
        // Handle the error response here
        console.error('Registration error:', error);
      }
    );
  }
}