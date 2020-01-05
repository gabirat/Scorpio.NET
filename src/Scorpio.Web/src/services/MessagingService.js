import * as SignalR from "@microsoft/signalr";
import { MessagePackHubProtocol } from "@microsoft/signalr-protocol-msgpack";
import { API } from "../constants/appConstants";
import LogService from "./LogService";

class MessagingService {
  constructor() {
    this._connection = null;
    this._watchdogInterval = null;
    // this._recoverConnection = this._recoverConnection.bind(this);
  }

  // Subscribe for given topic. If any messages appear on it, the handler will be called with received data.
  subscribe(topic, handler) {
    if (typeof handler !== "function" || typeof topic !== "string") return;

    if (this._connection) {
      LogService.info(`SignalR: subscribed to ${topic}`);
      this._connection.on(topic, data => {
        handler(data);
      });
    }
  }

  unsubscribe(topic, handler) {
    if (typeof handler !== "function" || typeof topic !== "string") return;

    if (this._connection) {
      LogService.info(`SignalR: unsubscribed ${topic}`);
      this._connection.off(topic, handler);
    }
  }

  // Send message to given topic.
  send(topic, message) {
    if (!this._connection || this._connection.connectionState !== "Connected") {
      LogService.error("SignalR: send message request, bot state is not connected");
      return;
    }

    this._connection.send(topic, message);
  }

  // Starts the connection. Returns promise (awaitable).
  async connectAsync() {
    if (this._connection && this._connection.connectionState === "Connected") {
      LogService.info("SignalR: trying to connect, but already connected, abadoning.");
      return;
    }

    this._watchdogInterval = setInterval(this._recoverConnection, 2000);

    const endpoint = API.SIGNALR;
    this._connection = new SignalR.HubConnectionBuilder()
      .withUrl(endpoint)
      .withHubProtocol(new MessagePackHubProtocol())
      .configureLogging(SignalR.LogLevel.Debug)
      .withAutomaticReconnect([0, 1000, 3000, 6000])
      .build();

    window.scorpioMessaging.signalR = this._connection;

    this._setup();

    await this._connection.start();
  }

  _recoverConnection = async () => {
    if (this._connection && this._connection.connectionState === "Disconnected") {
      await this.connectAsync();
    }
  };

  // Stops the connection with server.
  disconnect() {
    this._connection.stop();
    window.scorpioMessaging.signalR = null;
    if (this._watchdogInterval) {
      clearInterval(this._watchdogInterval);
    }
  }

  _setup() {
    this._connection.onreconnecting(err => {
      LogService.info("SignalR reconnecting: ", err);
    });

    this._connection.onreconnected(connId => {
      LogService.info("SignalR reconnected: ", connId);
    });

    this._connection.onclose(err => {
      LogService.error("SignalR errored", err);
      this._errorHandler(err);
    });
  }

  _errorHandler = error => {
    LogService.error(error);
    this.connectAsync();
  };
}

const singleton = new MessagingService();
export default singleton;
