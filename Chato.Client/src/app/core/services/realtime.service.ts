import { Injectable } from '@angular/core'
import * as signalR from '@microsoft/signalr'
import { ChatStore } from '../store/chat.store'
import { SAVED_TOKEN_KEY } from '../helpers/consts'
import { Chat, ChatMessage } from '../models/chat.models'
import { AuthenticationService } from './auth.service'
import { filter, first, firstValueFrom, Subject } from 'rxjs'

@Injectable({
    providedIn: 'root'
})
export class ChattoHubService {

    private hubConnection: signalR.HubConnection | undefined

    private isConnected = false
    private isConnectedSubj = new Subject<boolean>()

    constructor(private chatStore: ChatStore,
                private auth: AuthenticationService) {
    }

    public startConnection() {
        this.hubConnection = new signalR.HubConnectionBuilder()
            .withUrl('https://localhost:7138/rtxrazgavor', {
                withCredentials: true,
                accessTokenFactory: () => sessionStorage.getItem(SAVED_TOKEN_KEY)
            }).build()

        this.hubConnection
            .start()
            .then(() => {
                console.log('Connected to rtxrazgavor')
                this.isConnectedSubj.next(true)
            })
            .catch(err => console.log('Error while starting connection: ' + err))

        this.hubConnection.on('SendTextToChat', (message: ChatMessage) => {
            console.log('SendTextToChat', message)

            const chat = this.chatStore.selectedChat()

            if (!chat.messages) {
                chat.messages = []
            }

            chat.messages.push({
                ...message,
                isSelf: this.auth.user().userName == message.fromUser
            })
        })

        this.hubConnection.on('SendText', (fromUser, message) => {
            console.log('SendText', fromUser, message)
        })
    }

    public sendMessageToOthersInChat(message: ChatMessage) {
        this.hubConnection?.invoke('SendMessageToOthersInChat', message)
            .catch(err => console.error(err))
    }

    public downloadHistory(chat: Chat) {

        if (!chat.messages) {
            chat.messages = []
        }

        const stream = this.hubConnection.stream('DownloadHistory', chat.chatName)

        stream.subscribe({
            next: (messageInfo: ChatMessage) => {
                chat.messages.push({
                    ...messageInfo,
                    isSelf: this.auth.user().userName == messageInfo.fromUser
                })
            },
            complete: () => {
                console.log('Downloading messaged completed')
            },
            error: (err: any) => {
                console.error('Error while streaming messages:', err)
            }
        })
    }

    public joinOnCreateChat(chatName: string) {
        this.hubConnection?.invoke('JoinOrCreateChat', chatName)
            .catch(err => console.error(err))
    }

    // ... (Other methods for JoinOrCreateChat, LeaveGroup, etc.)

    // helper method to ensure signalR is connected before invoking methods on it
    public async invokeWhenConnected(task: () => void) {
        if (this.isConnected) {
            await this.sleep(200)
            task()
        } else {
            await firstValueFrom(this.isConnectedSubj.pipe(filter(connected => connected), first()))
            await this.sleep(200)
            task()
        }
    }

    sleep = (ms) => {
        return new Promise(resolve => setTimeout(resolve, ms))
    }
}
