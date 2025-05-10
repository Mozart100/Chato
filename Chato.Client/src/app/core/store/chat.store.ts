import { computed, Injectable, signal, WritableSignal } from '@angular/core'
import { Chat } from '../models/chat.models'
import { AuthenticationService } from '../services/auth.service'

@Injectable({ providedIn: 'root' })
export class ChatStore {

    private readonly forceUpdate = signal(0)

    constructor(private auth: AuthenticationService) {
    }

    allChats: WritableSignal<Chat[]> = signal([])
    selectedChat: WritableSignal<Chat | null> = signal(null)

    memberOfCurrentChat = computed(() => {
        this.forceUpdate();
        const chat = this.selectedChat();
        const res = chat.users.includes(this.auth.user().userName);
        return res;
    })

    addUserToChat(userName: string, chatName: string) {
        this.allChats.update(chats => {
            const chat = chats.find(c => c.roomName == chatName)
            if (chat && !chat.users.includes(userName)) {
                chat.users = [...chat.users, userName]
            }
            return chats
        })

        this.forceUpdateComputed()
    }

    private forceUpdateComputed() {
        this.forceUpdate.set(new Date().getTime())
    }

    selectChat(chatName : string):Chat {
        var chats = this.allChats();
        const chat = chats.find(chat => chat.roomName == chatName)
        if (chat) {
            this.selectedChat.set(chat);
        }

        return chat;
    }
}
