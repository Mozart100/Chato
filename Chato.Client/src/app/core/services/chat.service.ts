import { Injectable } from '@angular/core'
import { BaseApiService } from './base-api.service'
import { HttpClient } from '@angular/common/http'
import { ToastrService } from 'ngx-toastr'
import { environment } from '../../../environments/environment'
import { ChatsDto } from '../models/dto'
import { ChatStore } from '../store/chat.store'
import { ChattoHubService } from './realtime.service'
import { Chat } from '../models/chat.models'
import { AuthenticationService } from './auth.service'

const LOAD_ROOMS_URL = '/api/Chat/all'

@Injectable({ providedIn: 'root' })
export class ChatService extends BaseApiService {

    private readonly apiUrl: string

    constructor(http: HttpClient,
                alert: ToastrService,
                private chatStore: ChatStore,
                private realtime: ChattoHubService,
                private auth: AuthenticationService) {
        super(http, alert)
        this.apiUrl = environment.apiUrl
    }

    loadAllRooms() {
        this.sendGet<ChatsDto>(this.apiUrl + LOAD_ROOMS_URL)
            .then(data => {
                console.log('Chats response', data.Body.rooms)
                const rooms = data.Body.rooms.map(chat => ({
                    ...chat,
                    messages: []
                }))
                console.log('Filtered rooms', rooms)
                this.chatStore.allChats.set(rooms)

                const lobby = data.Body.rooms.find(chat => chat.roomName == 'lobi')
                if (lobby) {
                    this.chatStore.selectedChat.set(lobby)
                    this.realtime.invokeWhenConnected(() => this.realtime.downloadHistory(lobby))
                }
            })
    }

    addNewChat(chatName: string) {
        const newChat: Chat = {
            roomName: chatName,
            messages: [],
            users: [this.auth.user().userName]
        }
        this.chatStore.allChats.update(chats => {
            chats.unshift(newChat)
            return chats
        })
        this.chatStore.selectedChat.set(newChat)
    }
}
