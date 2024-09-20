import { computed, Injectable, Signal, signal, WritableSignal } from '@angular/core'
import { ChatMessage, Chat } from '../models/chat.models'
import { AuthenticationService } from '../services/auth.service'

@Injectable({ providedIn: 'root' })
export class ChatStore {

    constructor(private auth: AuthenticationService) {
    }

    allRooms: WritableSignal<Chat[]> = signal([])
    selectedRoom: WritableSignal<Chat | null> = signal(null)

    currentMessages: Signal<ChatMessage[]> = computed(() => {
        return (this.selectedRoom().messages || [])
            .map(roomMsg => ({
                userName: roomMsg.userName,
                message: roomMsg.message,
                isSelf: roomMsg.userName == this.auth.user()?.userName
            }))
    })
}
