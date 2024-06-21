import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';

import {
  RegistrationRequest,
  RegistrationResponse,
} from '../Models/RegistratioRequest';
import { Observable } from 'rxjs';
import { ServiceBase } from './ServiceBase';

@Injectable({
  providedIn: 'root',
})
export class LoginService extends ServiceBase {
  constructor(httpClient: HttpClient) {
    super(httpClient);
  }

  resistrationUser(
    userName: string,
    password: string
  ): Observable<RegistrationResponse> {
    const url = `${this.baseUrl}/api/Auth/register`; // Adjust endpoint as needed
    const body = { userName, password,age:18,description:"xdsdf",gender:"male"};
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });

    return this.httpClient.post<RegistrationResponse>(url, body, { headers });
  }
}
