import { Component, OnInit } from '@angular/core';
import { BehaviorSubject, Observable, filter, tap } from 'rxjs';
import { RoomService } from '../service/room-service.service';
import { ChatRoomDto } from '../Models/GetAllRoomResponse';

@Component({
  selector: 'app-sidebar-rooms',
  templateUrl: './sidebar-rooms.component.html',
  styleUrls: ['./sidebar-rooms.component.scss'],
})
export class SidebarRoomsComponent implements OnInit {
  rooms$: Observable<ChatRoomDto[]>;

  constructor(private _roomService: RoomService) {
    // const tmp =fb.group()
    this.rooms$ = this._roomService.rooms$;
  }

  ngOnInit(): void {
    this._roomService.getRooms();
  }

  roomSelected(room: ChatRoomDto): void {
    
  }
}
