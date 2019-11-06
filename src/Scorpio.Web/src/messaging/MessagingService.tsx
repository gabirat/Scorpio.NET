import * as SignalR from "@microsoft/signalr";
import { MessagePackHubProtocol } from "@microsoft/signalr-protocol-msgpack";

class MessagingService {
  private static _instance: MessagingService;
  private _isConnected: boolean;
  private _connection: SignalR.HubConnection | undefined;

  private constructor() {
    this._isConnected = false;
  }

  public message: any = new Array<number>();

  static getInstance(): MessagingService {
    if (!MessagingService._instance) {
      MessagingService._instance = new MessagingService();
    }
    return MessagingService._instance;
  }

  public send(topic: string, message: any): void {
    if (!this._isConnected || this._connection === undefined) return;

    this._connection.send(topic, message);
  }

  public async connect(): Promise<void> {
    if (this._isConnected) return;

    this._connection = new SignalR.HubConnectionBuilder()
      .withUrl("http://localhost:8080/hub/main")
      .withHubProtocol(new MessagePackHubProtocol())
      .configureLogging(SignalR.LogLevel.Debug)
      .withAutomaticReconnect([0, 1000, 30000, 6000])
      .build();

    this.setup();

    await this._connection.start();
    this._isConnected = true;
  }

  public disconnect(): void {
    this._connection!.stop().then(_ => (this._isConnected = false));
  }

  private setup(): void {
    this._connection!.on("data", (data: any) => {
      this.message = data;
      console.log(data);
    });

    this._connection!.onreconnecting((err: Error | undefined) => {
      console.warn("SignalR reconnecting: ", err);
    });

    this._connection!.onreconnected((connId: string | undefined) => {
      console.log("SignalR reconnected: ", connId);
    });

    this._connection!.onclose(err => {
      console.error("SignalR errored");
      this.errorHandler(err);
    });
  }

  private errorHandler(error: Error | undefined): void {
    console.error(error);
  }
}

export default MessagingService;
