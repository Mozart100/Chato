import { HttpClient } from '@angular/common/http';


export class HttpServiceBase {
  protected baseUrl: string;
  constructor(protected httpClient: HttpClient) {
    this.baseUrl = 'https://localhost:7138';
  }
}
