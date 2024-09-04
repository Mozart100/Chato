import { Component, OnInit } from '@angular/core'
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms'
import { ActivatedRoute, Router } from '@angular/router'

import { AuthenticationService } from '../../../core/services/auth.service'

import { TranslateService } from '@ngx-translate/core'
import { firstValueFrom } from 'rxjs'

@Component({
    selector: 'app-login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.scss']
})

/**
 * Login component
 */
export class LoginComponent implements OnInit {

    loginForm: UntypedFormGroup
    submitted = false
    error = ''
    returnUrl: string

    constructor(private formBuilder: UntypedFormBuilder,
                private route: ActivatedRoute,
                private router: Router,
                private auth: AuthenticationService,
                private translate: TranslateService) {
    }


    ngOnInit(): void {
        this.loginForm = this.formBuilder.group({
            username: ['Michael', [Validators.required, Validators.min(3)]],
            description: ['Ohhh, wanna dance with somebody', [Validators.required, Validators.min(3)]],
            age: [40, [Validators.required, Validators.min(18)]],
            gender: ['', []],
        })

        // fill gender placeholder
        firstValueFrom(this.translate.get('login.form.gender.placeholder'))
            .then(text => this.loginForm.controls.gender.setValue(text))

        // reset login status
        // this.authenticationService.logout();
        // get return url from route parameters or default to '/'
        // tslint:disable-next-line: no-string-literal
        this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/'
    }

    // convenience getter for easy access to form fields
    get f() {
        return this.loginForm.controls
    }

    /**
     * Form submit
     */
    onSubmit() {
        this.submitted = true

        if (this.loginForm.invalid) { // stop here if form is invalid
            return
        } else {
            this.auth.register({
                Age: this.loginForm.controls.age.value,
                Description: this.loginForm.controls.description.value,
                Gender: this.loginForm.controls.gender.value,
                UserName: this.loginForm.controls.username.value
            }).then(() => {
                this.router.navigate([this.returnUrl])
            })
        }
    }
}
