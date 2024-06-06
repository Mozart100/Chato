import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { RegistrationRequest,RegistrationResponse } from '../Models/RegistratioRequest';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class LoginService {
  private baseUrl: string;

  constructor(private httpClient: HttpClient) {
    this.baseUrl = 'https://localhost:7138';
  }

  resistrationUser(userName: string, password: string): Observable<RegistrationResponse> {
    
    const url = `${this.baseUrl}/api/Auth/register`;  // Adjust endpoint as needed
    const body = { userName, password };
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });

    return this.httpClient.post<RegistrationResponse>(url, body, { headers });
  }

  


}
