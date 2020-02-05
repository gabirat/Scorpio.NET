import React, { useEffect, useState } from "react";
import { Segment, Icon } from "semantic-ui-react";
import MessagingService from "../../services/MessagingService";

const SignleState = ({ resource, isOk }) => {
  return (
    <div className="text-large" style={{ marginTop: "4px" }}>
      <Icon color={isOk ? "green" : "red"} name="heartbeat"></Icon>
      {resource}: <span className={isOk ? "ok" : "nok"}>{isOk ? "OK" : "NOK"}</span>
    </div>
  );
};

const Statusoverlay = () => {
  const [isSignalrOk, setSignalrOk] = useState(false);

  useEffect(() => {
    const signalRHandler = state => {
      setSignalrOk(state === "Connected");
    };
    MessagingService.subscribeConnectionChange(signalRHandler);

    return () => {
      MessagingService.unSubscribeConnectionChange(signalRHandler);
    };
  }, []);

  return (
    <Segment className="scorpio-status-overlay-container">
      <SignleState resource={"SignalR"} isOk={isSignalrOk} />
      <SignleState resource={"RabbitMQ"} isOk={false} />
      <SignleState resource={"MongoDB"} isOk={true} />
    </Segment>
  );
};

export default Statusoverlay;
