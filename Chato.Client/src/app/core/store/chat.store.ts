import { computed, Injectable, Signal, signal, WritableSignal } from '@angular/core'
import { ChatMessage, Room } from '../models/chat.models'
import { AuthenticationService } from '../services/auth.service'

@Injectable({ providedIn: 'root' })
export class ChatStore {

    constructor(private auth: AuthenticationService) {
    }

    allRooms: WritableSignal<Room[]> = signal([])
    selectedRoom: WritableSignal<Room | null> = signal(null)

    currentMessages: Signal<ChatMessage[]> = computed(() => {
        return (this.selectedRoom().messages || [])
            .map(roomMsg => ({
                userName: roomMsg.userName,
                message: roomMsg.message,
                isSelf: roomMsg.userName == this.auth.user()?.userName
            }))
    })
}
