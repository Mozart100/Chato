export interface Room {
    roomName: string
    senderInfo: { 'userName': 'string', 'message': 'string' }[],
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
