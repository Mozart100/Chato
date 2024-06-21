export interface ChatRoomDto {
  roomName: string;
  senderInfo: SenderInfo[] | undefined;
  users: string[] | undefined;
}

export interface GetAllRoomResponse {
  rooms: ChatRoomDto[] | undefined;
}

export interface SenderInfo {
  userName: string;
  message: string;
}
