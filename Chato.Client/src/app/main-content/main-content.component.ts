import { Component, OnInit } from '@angular/core';
import { BehaviorSubject, Observable, filter, tap } from 'rxjs';
import { RoomService } from '../service/room-service.service';
import { ChatRoomDto } from '../generated/Dtos';
import { SignalrListenerService } from '../service/signalr-listener.service';
import { LoginService } from '../service/login-service.service';
import AppConsts from '../Consts/AppConsts';

@Component({
  selector: 'app-main-content',
  templateUrl: './main-content.component.html',
  styleUrls: ['./main-content.component.scss'],
})
export class MainContentComponent implements OnInit{
  
  message:string="";
  UserName="anatoliy38";


  constructor(private _loginService:LoginService, private _signalrListenerService:SignalrListenerService) {
    
  }

  ngOnInit(): void {
    
    this.UserName = this.generateRandomNumber() + this.UserName;
    this._loginService.resistrationUser(this.UserName).subscribe(res=>{
      console.log("app.component - registration response received session"+ res.token!);
      sessionStorage.setItem(AppConsts.Token,res.token!);
    });
  }

  generateRandomNumber(): number {
    return Math.floor(Math.random() * 10000) + 1;
  }

  
  onClicked(){
    this._signalrListenerService.sendMessage(this.UserName,this.message);
  }

}
