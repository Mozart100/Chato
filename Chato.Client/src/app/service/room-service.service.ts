import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { GetAllRoomResponse } from '../Models/GetAllRoomResponse';
import { Observable } from 'rxjs';
import { ServiceBase } from './ServiceBase';

@Injectable({
  providedIn: 'root'
})
export class RoomService extends ServiceBase {

  constructor( httpClient: HttpClient) {
    super(httpClient);
  }


  getRooms():Observable<GetAllRoomResponse>
  {
      const url = `${this.baseUrl}/api/room`;  
      const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
  
      return this.httpClient.get<GetAllRoomResponse>(url,{ headers });
  }
}
