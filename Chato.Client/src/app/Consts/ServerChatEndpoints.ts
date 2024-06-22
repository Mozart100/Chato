export interface IChattobEndpoints {
    // BroadcastMessage(fromUser: string, message: string): void;
    // SendMessageToOtherUser(fromUser: string, toUser: string, message: string): void;

    BroadcastMessage( fromUser:string, message : string):void;
    ReplyMessage( fromUser:string, message:string):void;
    SendMessageToOtherUser(fromUser :string, toUser:string ,  ptr:string):void;
    SendMessage(fromUser:string,message:string):void;
}


export function getMemberName<T>(member: keyof T): string {
    return member as string;
}


// Helper function to get the name of the property
export function getMemberChattobEndpointsName<k  extends keyof IChattobEndpoints>(member: k): string {
    return member as string;
}

// // Usage example
// const methodName = getMemberChattobEndpointsName<IChattobEndpoints>('BroadcastMessage');
// console.log(methodName); // Outputs: "BroadcastMessage"