export interface IChattobEndpoints {
  BroadcastMessage(fromUser: string, message: string): void;
  SendText(fromUser: string, message: string): void;
  SelfReplay(message: string): void;
  SendMessageToOthersInGroup(group:string,  fromUser:string,  message:string):void
}



export function ChattoEndpoint<k extends keyof IChattobEndpoints>(
  member: k
): string {
  return member as string;
}
