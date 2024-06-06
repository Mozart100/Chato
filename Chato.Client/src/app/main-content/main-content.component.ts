import { Component, OnInit } from '@angular/core';
import { BehaviorSubject, Observable, filter, tap } from 'rxjs';
import { RoomService } from '../service/room-service.service';
import { ChatRoomDto } from '../Models/GetAllRoomResponse';

@Component({
  selector: 'app-main-content',
  templateUrl: './main-content.component.html',
  styleUrls: ['./main-content.component.scss'],
})
export class MainContentComponent implements OnInit {
  private roomsSubject: BehaviorSubject<ChatRoomDto[]>;
  
  rooms$: Observable<ChatRoomDto[]>;

  constructor(private _roomService: RoomService) {
    // const tmp =fb.group()
    this.roomsSubject = new BehaviorSubject<ChatRoomDto[]>([]);
    this.rooms$ = this.roomsSubject.asObservable();
  }

  ngOnInit() {
    this._roomService
      .getRooms()
      .pipe(
        tap((res) => {
          if (res && res.rooms) {
            this.roomsSubject.next(res.rooms);
          }
        })
      )
      .subscribe(
        (response) => {
          // Handle the response, e.g., assign it to a component property
          console.log('Rooms:', response);
        },
        (error) => {
          // Handle any errors
          console.error('Error fetching rooms:', error);
        }
      );
  }
}
