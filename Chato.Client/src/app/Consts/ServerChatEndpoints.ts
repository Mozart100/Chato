export interface IChattobEndpoints {
  BroadcastMessage(fromUser: string, message: string): void;
  // SendMessageToOtherUser(fromUser: string, toUser: string, ptr: string): void;
  // SendMessage(fromUser: string, message: string): void;
  // SendText(fromUser: string, message: string): void;
  // SendAll(fromUser: string, message: string): void;

  SendText(fromUser:string ,  message:string):void;
  SelfReplay(message:string):void;
}

export function getMemberName<T>(member: keyof T): string {
  return member as string;
}

export function ToChattobEndpoint<k extends keyof IChattobEndpoints>(
  member: k
): string {
  return member as string;
}
