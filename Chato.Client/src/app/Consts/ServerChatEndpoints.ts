export interface IChattobEndpoints {
    BroadcastMessage(fromUser: string, message: string): void;
    SendMessageToOtherUser(fromUser: string, toUser: string, message: string): void;
}


export function getMemberName<T>(member: keyof T): string {
    return member as string;
}


// Helper function to get the name of the property
export function getMemberChattobEndpointsName<IChattobEndpoints>(member: keyof IChattobEndpoints): string {
    return member as string;
}

// // Usage example
// const methodName = getMemberChattobEndpointsName<IChattobEndpoints>('BroadcastMessage');
// console.log(methodName); // Outputs: "BroadcastMessage"