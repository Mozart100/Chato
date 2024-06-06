import { Component, OnInit } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { RoomService } from '../service/room-service.service';

@Component({
  selector: 'app-main-content',
  templateUrl: './main-content.component.html',
  styleUrls: ['./main-content.component.scss'],
})
export class MainContentComponent implements OnInit {
  

  constructor(private _roomService:RoomService) {
    // const tmp =fb.group()
  }

  ngOnInit() {
    this._roomService.getRooms().pipe(
      tap(res => console.log(res))
    ).subscribe(
      response => {
        // Handle the response, e.g., assign it to a component property
        console.log('Rooms:', response);
      },
      error => {
        // Handle any errors
        console.error('Error fetching rooms:', error);
      }
    );
  }

  
}
