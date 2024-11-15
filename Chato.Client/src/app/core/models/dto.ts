import { Chat } from './chat.models'

export interface ChatsDto {
    Body: {
        rooms: Chat[]
    }
}

export interface ImagesResponseDto {
    Body: {
        files: string[]
    }
}
