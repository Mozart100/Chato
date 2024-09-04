import { Room } from './chat.models'

export interface RoomsDto {
    Body: {
        rooms: Room[]
    }
}
