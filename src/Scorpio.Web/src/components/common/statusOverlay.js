import React, { useEffect, useState } from "react";
import { Segment, Icon } from "semantic-ui-react";
import MessagingService from "../../services/MessagingService";
import { genericApi } from "../../api/genericApi";
import { API } from "../../constants/appConstants";

const SignleState = ({ resource, isOk }) => {
  return (
    <div className="text-large" style={{ marginTop: "4px" }}>
      <Icon color={isOk ? "green" : "red"} name="heartbeat"></Icon>
      {resource}:<span className={isOk ? "ok" : "nok"}>{isOk ? "OK" : "NOK"}</span>
    </div>
  );
};

const Statusoverlay = () => {
  const [isSignalrOk, setSignalrOk] = useState(false);
  const [apiStatuses, setApiStatuses] = useState([]);

  useEffect(() => {
    // When mount
    const signalRHandler = state => {
      setSignalrOk(state === "Connected");
    };

    const pollApiStatuses = async () => (await genericApi(API.HEALTH, "GET")).body;

    const pollerTask = setInterval(async () => {
      const response = await pollApiStatuses();
      if (response && response.results) {
        const dupa = Object.keys(response.results).map(key => {
          const val = response.results[key];
          return { resource: key, isOk: val.isHealthy };
        });
        setApiStatuses(dupa);
      }
    }, 1000);
    MessagingService.subscribeConnectionChange(signalRHandler);

    // When unmount
    return () => {
      clearInterval(pollerTask);
      MessagingService.unSubscribeConnectionChange(signalRHandler);
    };
  }, []);

  return (
    <Segment className="scorpio-status-overlay-container">
      <SignleState resource={"SignalR"} isOk={isSignalrOk} />
      {apiStatuses.map(x => {
        return <SignleState key={x.resource} resource={x.resource} isOk={x.isOk} />;
      })}
    </Segment>
  );
};

export default Statusoverlay;
