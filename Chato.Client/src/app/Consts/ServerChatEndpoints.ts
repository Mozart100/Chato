// interface IChattobEndpoints {
//     BroadcastMessage(fromUser: string, message: string): Promise<void>;
//     Downloads(downloadInfo: HubDownloadInfo, cancellationToken: CancellationToken): AsyncIterableIterator<Uint8Array>;
//     GetGroupHistory(roomName: string, cancellationToken: CancellationToken): AsyncIterableIterator<SenderInfo>;
//     JoinGroup(roomName: string): Promise<void>;
//     LeaveGroup(groupName: string): Promise<void>;
//     OnConnectedAsync(): Promise<void>;
//     RemoveChatHistory(groupName: string): Promise<void>;
//     SendMessageToOthersInGroup(group: string, fromUser: string, message: string): Promise<void>;
//     SendMessageToOtherUser(fromUser: string, toUser: string, message: string): Promise<void>;
//     UserDisconnectAsync(): Promise<void>;
// }