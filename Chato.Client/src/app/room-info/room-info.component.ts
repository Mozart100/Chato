import { Component } from '@angular/core';
import { SignalrListenerService } from '../service/signalr-listener.service';

@Component({
  selector: 'app-room-info',
  templateUrl: './room-info.component.html',
  styleUrls: ['./room-info.component.scss']
})
export class RoomInfoComponent {

    constructor(private _signalR:SignalrListenerService)
    {

    }

    


}
