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
    Body: {
        userName: string
        token: string
        age: number
        description: string
        gender: string
    }
}