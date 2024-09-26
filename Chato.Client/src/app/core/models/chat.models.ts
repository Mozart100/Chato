export interface Chat {
    chatName: string
    messages: ChatMessage[],
    users: string[]
}

export interface User {
    id: string
    userName: string
    age: number
    description: string
    gender: string
    rooms: string[],
    connectionId: string
}

export interface ChatMessage {
    chatName: string
    fromUser: string
    textMessage?: string
    image?: string
    isSelf: boolean
}

