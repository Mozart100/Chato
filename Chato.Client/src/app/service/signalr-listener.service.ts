import { Injectable, OnInit } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import AppConsts from '../Consts/AppConsts';
import { getMemberChattobEndpointsName } from '../Consts/ServerChatEndpoints';
import { debug } from 'console';

@Injectable({
  providedIn: 'root',
})
export class SignalrListenerService implements OnInit {
  private _connection: signalR.HubConnection;
  

  constructor() {


    debugger;
    this._connection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:7138/chat', {
        accessTokenFactory: () => this.getToken(),
      })
      .build();

    const ptr = getMemberChattobEndpointsName("BroadcastMessage");
    debugger;
    this._connection.on(
      getMemberChattobEndpointsName('BroadcastMessage'),
      (user: string, message: string) => {
        debugger;
        // const decodedMessage = atob(message);
        console.log(`Received from ${user} this message: ${message}`);
      }
    );

    this._connection.on(getMemberChattobEndpointsName('ReplyMessage'),(fromUser:string,message:string)=>{
      debugger;
      console.log(`Received from ${fromUser} this message: ${message}`);

    })
    
    const ptr2 = getMemberChattobEndpointsName('SendMessageToOtherUser');
    debugger; 
    this._connection.on(ptr2,
      (user: string, message: string) => {
        debugger;
        // const decodedMessage = atob(message);

        console.log(`Received from ${user} this message: ${message}`);
      }
    );

    // this.startConnection();
  }

  ngOnInit(): void {
    // Add any additional initialization logic
  }

  private getToken(): string {
    const token = sessionStorage.getItem(AppConsts.Token) || '';
    debugger;
    return token;
  }

  public startConnection(): void {
    this._connection
      .start()
      .then(() => {
        console.log('Connection started');
      })
      .catch((err) => console.log('Error while starting connection: ' + err));
  }

  public sendMessage(user: string, message: string): void {

    debugger;
    this._connection
      .invoke(getMemberChattobEndpointsName('BroadcastMessage'), user, message)
      .catch((err) => console.error('Error while sending message: ' + err));
  }
}
