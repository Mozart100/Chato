import { Component, OnInit } from '@angular/core';
import { BehaviorSubject, Observable, filter, tap } from 'rxjs';
import { RoomService } from '../service/room-service.service';
import { ChatRoomDto } from '../Models/GetAllRoomResponse';

@Component({
  selector: 'app-main-content',
  templateUrl: './main-content.component.html',
  styleUrls: ['./main-content.component.scss'],
})
export class MainContentComponent{
}
