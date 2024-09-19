import { Chat } from './chat.models'

export interface ChatsDto {
    Body: {
        rooms: Chat[]
    }
}
