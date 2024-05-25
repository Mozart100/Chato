import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-main-content',
  templateUrl: './main-content.component.html',
  styleUrls: ['./main-content.component.scss']
})
export class MainContentComponent implements OnInit {
  
  signupForm!: FormGroup;
  userNamesInvalid = ['xxx', 'kkk'];

  ngOnInit(): void {
    this.signupForm = new FormGroup({
      userName: new FormControl(null, [
        Validators.required,
        this.forbiddenNames.bind(this),
      ]),
      email: new FormControl(null, [Validators.required, Validators.email])
      // async doesnt work
      // asyncValidators: [this.forbiddenEmails.bind(this)],
      // updateOn: 'blur', 
    });

    this.signupForm.valueChanges.subscribe(val=>console.log(val));
    this.signupForm.statusChanges.subscribe(val=>console.log(val));
    this.signupForm.setValue({'userName':"toli","email":"aaa@aa.com"});
    
  }

  onSubmit() {
    console.log(this.signupForm);
    this.signupForm.reset();
  }

  forbiddenNames(control: FormControl): { [key: string]: boolean } | null {
    if (this.userNamesInvalid.indexOf(control.value) >= 0) {
      return { xxx: true };
    }

    return null;
  }

  

  forbiddenEmails(control: FormControl): Promise<any> | Observable<any> {
    const promise = new Promise((resolve, reject) => {
      setTimeout(() => {
        if (control.value === "xxx@xxx.com") {
          resolve({ 'emailxxx': true });
        } else {
          resolve(null);
        }
      }, 3000);
    });
  
    return promise;
  }

}
