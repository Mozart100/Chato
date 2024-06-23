export interface RegistrationRequest {
    userName?: string ;
}

export interface RegistrationResponse {
    userName?: string ;
    token?: string ;
    tokenCreated?: Date;
    tokenExpires?: Date;
}