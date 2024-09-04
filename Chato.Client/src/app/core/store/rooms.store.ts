import { Injectable, signal, WritableSignal } from '@angular/core'
import { Room } from '../models/chat.models'

@Injectable({ providedIn: 'root' })
export class RoomsStore {
    allRooms: WritableSignal<Room[]> = signal([])
}
