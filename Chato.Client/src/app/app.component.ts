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
  message:string="";


  UserName="anatoliy38";

  /**
   *
   */
  constructor(private matDialog:MatDialog, private _loginService:LoginService, private _signalrListenerService:SignalrListenerService) {
    
  }

  ngOnInit(): void {
    // this.matDialog.open(DialogBodyComponent,{width:"350pxs"});

    this.UserName = this.generateRandomNumber() + this.UserName;
    this._loginService.resistrationUser(this.UserName).subscribe(res=>{
      console.log("app.component - registration response received session"+ res.token!);
      sessionStorage.setItem(AppConsts.Token,res.token!);
    });

  }

  generateRandomNumber(): number {
    return Math.floor(Math.random() * 10000) + 1;
  }

  openDialog()
  {
    // this.matDialog.open(DialogBodyComponent,{width:"350pxs"});
  }


  onClicked(){
    this._signalrListenerService.sendMessage(this.UserName,this.message);
  }
}
