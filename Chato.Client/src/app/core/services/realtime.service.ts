import { Injectable } from '@angular/core'
import * as signalR from '@microsoft/signalr'
import { ChatStore } from '../store/chat.store'
import { SAVED_TOKEN_KEY } from '../helpers/consts'
import { Chat, ChatMessage, ChatType, SenderInfoType } from '../models/chat.models'
import { AuthenticationService } from './auth.service'
import { filter, first, firstValueFrom, Subject } from 'rxjs'
import { TranslateService } from '@ngx-translate/core'
import { environment } from '../../../environments/environment'

@Injectable({
    providedIn: 'root'
})
export class ChattoHubService {

    private hubConnection: signalR.HubConnection | undefined

    private isConnected = false
    private isConnectedSubj = new Subject<boolean>()

    private usedJoinedText = ''

    constructor(private chatStore: ChatStore,
                private auth: AuthenticationService,
                private translate: TranslateService) {

        this.initTranslatedText()
    }

    private initTranslatedText() {
        firstValueFrom(this.translate.get('common.userJoined'))
            .then(joinedTxt => {
                this.usedJoinedText = joinedTxt
            })
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

        this.hubConnection.on('SendNotificationn', (chatName: string, userName: string) => {
            console.log('SendNotificationn', chatName, userName)
            this.chatStore.addUserToChat(userName, chatName)
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

        const stream = this.hubConnection.stream('DownloadHistory', chat.roomName)

        stream.subscribe({
            next: (messageInfo: ChatMessage) => {

                console.log('HISTORY MSESA', messageInfo)

                if (messageInfo.senderInfoType == SenderInfoType.Image) {
                    messageInfo.image = `${environment.apiUrl}/${messageInfo.image}`
                }

                let msgText = ""
                switch (messageInfo.senderInfoType) {
                    case SenderInfoType.TextMessage:
                        msgText = messageInfo.textMessage
                        break;
                    case SenderInfoType.Joined:
                        msgText = this.translate.instant('common.userJoined')
                        break;
                    case SenderInfoType.Created:
                        msgText = this.translate.instant('common.userCreatedGroup')
                        break;
                }

                console.log('PUSH')

                chat.messages.push({
                    ...messageInfo,
                    isSelf: this.auth.user().userName == messageInfo.fromUser,
                    textMessage: msgText,
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

    public createPublicGroup(name: string, description: string) {
        return this.hubConnection?.invoke('JoinOrCreateChat',
            name, ChatType.Public, description
        )
    }

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
