import React, { useEffect, useState } from "react";
import { Icon } from "semantic-ui-react";
import MessagingService from "../../services/MessagingService";
import { genericApi } from "../../api/genericApi";
import { API } from "../../constants/appConstants";

const SignleState = ({ resource, isOk }) => {
  return (
    <div className="text-large" style={{ marginTop: "4px" }}>
      <Icon color={isOk ? "green" : "red"} name="heartbeat"></Icon>
      {resource}: <span className={isOk ? "ok" : "nok"}>{isOk ? "OK" : "NOK"}</span>
    </div>
  );
};

const AppHealth = () => {
  const [isSignalrOk, setSignalrOk] = useState(false);
  const [apiStatuses, setApiStatuses] = useState([]);

  useEffect(() => {
    const pollApiStatuses = async () => (await genericApi(API.HEALTH, "GET")).body;
    const updateApiStatuses = async () => {
      if (MessagingService._connection && MessagingService._connection.connectionState === "Connected") {
        setSignalrOk(MessagingService._connection.connectionState === "Connected");
      }

      const response = await pollApiStatuses();
      if (response && response.results) {
        const dupa = Object.keys(response.results).map(key => {
          const val = response.results[key];
          return { resource: key, isOk: val.isHealthy };
        });
        setApiStatuses(dupa);
      }
    };

    updateApiStatuses();

    const pollerTask = setInterval(updateApiStatuses, 10000);

    // When unmount
    return () => {
      clearInterval(pollerTask);
    };
  }, []);

  return (
    <>
      <SignleState resource={"SignalR"} isOk={isSignalrOk} />
      {apiStatuses.map(x => {
        return <SignleState key={x.resource} resource={x.resource} isOk={x.isOk} />;
      })}
    </>
  );
};

export default AppHealth;
