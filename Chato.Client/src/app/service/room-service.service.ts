import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { GetAllRoomResponse } from '../Models/GetAllRoomResponse';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class RoomService {
  private baseUrl: string;

  constructor(private httpClient: HttpClient) {
    this.baseUrl = 'https://localhost:7138';
  }


  getRooms():Observable<GetAllRoomResponse>
  {
      const url = `${this.baseUrl}/api/room`;  
      const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
  
      return this.httpClient.get<GetAllRoomResponse>(url,{ headers });
  }
}
