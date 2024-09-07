import { Inject, Injectable, signal, WritableSignal } from '@angular/core'

import { RegistrationRequest, RegistrationResponse, User, UserResponse } from '../models/auth.models'
import { environment } from '../../../environments/environment'
import { BaseApiService } from './base-api.service'
import { HttpClient } from '@angular/common/http'
import { ToastrService } from 'ngx-toastr'
import { Router } from '@angular/router'
import { SAVED_TOKEN_KEY } from '../helpers/consts'
import { firstValueFrom } from 'rxjs'

const REGISTER_URL = '/api/Auth/Register'
const CHECK_AUTH_URL = '/api/Auth/status'
const LOAD_USER_URL = '/api/User'

@Injectable({ providedIn: 'root' })
export class AuthenticationService extends BaseApiService {

    private readonly apiUrl: string

    /**
     * Returns the current user
     */
    user: WritableSignal<User | null> = signal(null)

    constructor(http: HttpClient,
                alert: ToastrService,
                private router: Router) {
        super(http, alert)
        this.apiUrl = environment.apiUrl
    }

    /**
     * Sign-up user for a new session
     * @param request User registration data
     */
    public register(request: RegistrationRequest): Promise<void> {
        return this.sendPost<RegistrationRequest, RegistrationResponse>(this.apiUrl + REGISTER_URL, request)
            .then(data => {
                // set user
                this.user.set({
                    userName: data.Body.userName,
                    age: data.Body.age,
                    gender: data.Body.gender,
                    description: data.Body.description
                })
                // set token
                sessionStorage.setItem(SAVED_TOKEN_KEY, data.Body.token)
            })
    }

    public loadUser(): Promise<boolean> {
        return this.sendGet<UserResponse>(this.apiUrl + LOAD_USER_URL)
            .then(data => {
                this.user.set(data.Body.user)
                return true
            })
            .catch(() => false)
    }

    /**
     * Check if current user is authenticated
     */
    public checkAuth(): Promise<boolean> {
        return firstValueFrom(this.http.get(this.apiUrl + CHECK_AUTH_URL, { responseType: 'text' }))
            .then(data => true)
            .catch(() => false)
    }

    /**
     * Logout the user
     */
    logout(): void {
        this.router.navigate(['/account/login'])
        sessionStorage.removeItem(SAVED_TOKEN_KEY)
        this.user.set(null)
    }
}

