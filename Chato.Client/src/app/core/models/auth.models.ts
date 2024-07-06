export class User {
    username: string
    age: number
    gender: string
    description: string
}

export interface RegistrationRequest {
    UserName: string
    Age: number
    Description: string
    Gender: string
}

export interface RegistrationResponse {
    userName: string
    age: number
    description: string
    gender: string
    token: string
    tokenCreated: Date
    tokenExpires: Date
}
