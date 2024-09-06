export interface Room {
    roomName: string
    messages: { 'userName': string, 'message': string }[],
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
    userName: string
    message: string
    isSelf: boolean
}
