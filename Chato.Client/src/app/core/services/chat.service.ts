import { Injectable } from '@angular/core'
import { BaseApiService } from './base-api.service'
import { HttpClient } from '@angular/common/http'
import { ToastrService } from 'ngx-toastr'
import { environment } from '../../../environments/environment'
import { ChatsDto } from '../models/dto'
import { ChatStore } from '../store/chat.store'
import { ChattoHubService } from './realtime.service'

const LOAD_ROOMS_URL = '/api/Chat/all'

@Injectable({ providedIn: 'root' })
export class ChatService extends BaseApiService {

    private readonly apiUrl: string

    constructor(http: HttpClient,
                alert: ToastrService,
                private chatStore: ChatStore,
                private realtime: ChattoHubService) {
        super(http, alert)
        this.apiUrl = environment.apiUrl
    }

    loadAllRooms() {
        this.sendGet<ChatsDto>(this.apiUrl + LOAD_ROOMS_URL)
            .then(data => {
                console.log('Chats response', data.Body.rooms)
                this.chatStore.allChats.set(data.Body.rooms)

                const lobby = data.Body.rooms.find(chat => chat.chatName == 'lobi')
                if (lobby) {
                    this.chatStore.selectedChat.set(lobby)
                    this.realtime.invokeWhenConnected(() => this.realtime.downloadHistory(lobby))
                }
            })
    }
}
