import { Injectable, signal, WritableSignal } from '@angular/core'
import { Room } from '../models/chat.models'

@Injectable({ providedIn: 'root' })
export class ChatStore {
    allRooms: WritableSignal<Room[]> = signal([])
    selectedRoom: WritableSignal<Room | null> = signal(null)
}
