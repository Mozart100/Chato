import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { HttpServiceBase } from './ServiceBase';
import { ChatRoomDto, GetAllRoomResponse } from '../Models/GetAllRoomResponse';

@Injectable({
  providedIn: 'root',
})
export class RoomService extends HttpServiceBase {

  private _roomsSubject = new BehaviorSubject<ChatRoomDto[]>([]);
  rooms$: Observable<ChatRoomDto[]> = this._roomsSubject.asObservable();

  constructor(httpClient: HttpClient) {
    super(httpClient);
  }

  getRooms(): void {
    const url = `${this.baseUrl}/api/room`;
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });

    this.httpClient.get<GetAllRoomResponse>(url, { headers }).pipe(
      tap((response) => {
        console.log("Entered");
        if (response && response.rooms) {
          console.log("Response rooms:", response.rooms);
          const updatedRooms = [...this._roomsSubject.value, ...response.rooms];
          console.log("Updated rooms:", updatedRooms);
          this._roomsSubject.next(updatedRooms);
        }
      })
    ).subscribe();
  }
}
