import { computed, Injectable, signal, WritableSignal } from '@angular/core'
import { Chat } from '../models/chat.models'
import { AuthenticationService } from '../services/auth.service'

@Injectable({ providedIn: 'root' })
export class ChatStore {

    constructor(private auth: AuthenticationService) {
    }

    allChats: WritableSignal<Chat[]> = signal([])
    selectedChat: WritableSignal<Chat | null> = signal(null)

    memberOfCurrentChat = computed(() => {
        return this.selectedChat().users.includes(this.auth.user().userName)
    })
}
