import { Injectable, OnInit } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import AppConsts from '../Consts/AppConsts';

@Injectable({
  providedIn: 'root',
})
export class SignalrListenerService implements OnInit {
  private _connection: signalR.HubConnection;

  constructor() {
    this._connection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:7138/chat', {
        accessTokenFactory: () => this.getToken(),
      })
      .build();

    this._connection.on("SendMessageToOthers", (user: string, message: string) => {
  
    
      // const decodedMessage = atob(message);

      console.log(`Received from ${user} this message: ${message}`);

});

this._connection.on("SendMessage", (user: string, message: string) => {
  

  // const decodedMessage = atob(message);

  console.log(`Received from ${user} this message: ${message}`);

});



    this.startConnection();
  }

  ngOnInit(): void {
    // Add any additional initialization logic
  }

  private getToken(): string {
    const token =  localStorage.getItem(AppConsts.Token) || '';
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
    this._connection
      .invoke('SendMessageToOthers', user, message)
      .catch((err) => console.error('Error while sending message: ' + err));
  }
}
