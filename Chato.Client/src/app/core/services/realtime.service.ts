import { Injectable } from '@angular/core'
import * as signalR from '@microsoft/signalr'
import { ChatStore } from '../store/chat.store'
import { SAVED_TOKEN_KEY } from '../helpers/consts'

@Injectable({
    providedIn: 'root'
})
export class ChattoHubService {
    private hubConnection: signalR.HubConnection | undefined

    constructor(private chatStore: ChatStore) {
    }

    public startConnection() {
        this.hubConnection = new signalR.HubConnectionBuilder()
            .withUrl('https://localhost:7138/rtxrazgavor', {
                withCredentials: true,
                accessTokenFactory: () => {
                    const token =sessionStorage.getItem(SAVED_TOKEN_KEY)
                    return token
                }
            }) // Replace with your actual hub URL
            .build()

        this.hubConnection
            .start()
            .then(() => console.log('Connection started'))
            .catch(err => console.log('Error while starting connection: ' + err))

        this.hubConnection.on('SendTextToChat', (chat, fromUser, message) => {
            console.log('SendTextToChat', chat, fromUser, message)
        })

        this.hubConnection.on('SendText', (fromUser, message) => {
            console.log('SendText', fromUser, message)
        })
    }

    public sendMessageToOthersInGroup(chatName: string, fromUser: string, message: string) {
        this.hubConnection?.invoke('SendMessageToOthersInGroup', chatName, fromUser, message)
            .catch(err => console.error(err))
    }

    // ... (Add other methods for JoinOrCreateChat, LeaveGroup, etc.)
}
