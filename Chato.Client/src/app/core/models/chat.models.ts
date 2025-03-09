export interface Chat {
    roomName: string
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
    timeStemp: number
    senderInfoType: SenderInfoType
    textMessage?: string
    image?: string
    isSelf: boolean
}

export enum SenderInfoType {
    TextMessage = 1,
    Image = 2,
    Joined = 3,
    Created = 4
}

export enum ChatType {
    Public = 0,
    Private = 1
}
