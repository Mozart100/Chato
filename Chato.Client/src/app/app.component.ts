import { Component, OnInit } from '@angular/core';
import { DialogBodyComponent } from './dialog-body/dialog-body.component';
import { MatDialog } from '@angular/material/dialog';

import * as signalR from "@microsoft/signalr";
import { LoginService } from './service/login-service.service';
import AppConsts from './Consts/AppConsts';
import { SignalrListenerService } from './service/signalr-listener.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'Chato.Client';

  /**
   *
   */
  constructor(private matDialog:MatDialog) {
    
  }

  ngOnInit(): void {
    // this.matDialog.open(DialogBodyComponent,{width:"350pxs"});



  }


  openDialog()
  {
    // this.matDialog.open(DialogBodyComponent,{width:"350pxs"});
  }


}
