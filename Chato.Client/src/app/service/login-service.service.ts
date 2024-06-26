import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';

import {
  RegistrationRequest,
  RegistrationResponse,
} from '../generated/Dtos';
import { Observable } from 'rxjs';
import { HttpServiceBase } from './ServiceBase';

@Injectable({
  providedIn: 'root',
})
export class LoginService extends HttpServiceBase {
  constructor(httpClient: HttpClient) {
    super(httpClient);
  }

  resistrationUser(
    userName: string
  ): Observable<RegistrationResponse> {
    const url = `${this.baseUrl}/api/Auth/register`; // Adjust endpoint as needed
    const body:RegistrationRequest = { userName,age:18,description:"xdsdf",gender:"male"};
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });

    return this.httpClient.post<RegistrationResponse>(url, body, { headers });
  }
}
