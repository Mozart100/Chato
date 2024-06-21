import { HttpClient } from '@angular/common/http';


export class ServiceBase {
  protected baseUrl: string;
  constructor(protected httpClient: HttpClient) {
    this.baseUrl = 'https://localhost:7138';
  }
}
