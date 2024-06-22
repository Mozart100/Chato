import { Injectable, OnInit } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import AppConsts from '../Consts/AppConsts';
import { getMemberChattobEndpointsName } from '../Consts/ServerChatEndpoints';

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

    // Register SignalR event handlers
    this._connection.on(
      getMemberChattobEndpointsName('SendMessage'),
      (user: string, message: string) => {
        debugger;
        const decodedMessage = atob(message);
        console.log(`Received from ${user} this message: ${decodedMessage}`);
      }
    );

  
  }

  ngOnInit(): void {
    // Optionally start the connection here or call startConnection() from a component
    this.startConnection();
  }

  private getToken(): string {
    return sessionStorage.getItem(AppConsts.Token) || '';
  }

  public startConnection(): void {
    this._connection
      .start()
      .then(() => {
        console.log('SignalR connection started');
      })
      .catch((err) => console.log('Error while starting connection: ' + err));
  }

  public sendMessage(user: string, message: string): void {
    this._connection
      .send(getMemberChattobEndpointsName('BroadcastMessage'), user, "kukariku")
      .then(() => {
        console.log('Message sent');
      })
      .catch((err) => console.error('Error while sending message: ' + err));
  }
}
