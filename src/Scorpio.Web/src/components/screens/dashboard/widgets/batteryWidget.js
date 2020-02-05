import React, { useEffect, useState } from "react";
import { TOPICS } from "../../../../constants/appConstants";
import MessagingService from "../../../../services/MessagingService";
import LogService from "../../../../services/LogService";
import { Progress, Segment, Header } from "semantic-ui-react";

const BatteryWidget = () => {
  const [voltage, setVoltage] = useState(0);
  const [current, setCurrent] = useState(0);
  const [lastUpdated, setLastUpdated] = useState(null);

  const websocketHandler = data => {
    LogService.debug(`Battery widget received data`, data);

    try {
      const obj = JSON.parse(data);
      if (obj && obj.voltage && obj.current) {
        setLastUpdated(new Date());
        const parsedVotlage = Number.parseFloat(obj.voltage);
        if (!isNaN(parsedVotlage)) setVoltage(parsedVotlage);

        const parsedCurrent = Number.parseFloat(obj.current);
        if (!isNaN(parsedCurrent)) setCurrent(parsedCurrent);
      }
    } catch {}
  };

  useEffect(() => {
    MessagingService.subscribe(TOPICS.BATTERY_DATA, websocketHandler);

    return () => {
      MessagingService.unsubscribe(TOPICS.BATTERY_DATA, websocketHandler);
    };
  });

  return (
    <Segment basic>
      {voltage || current ? (
        <>
          <Header> Battery {voltage}</Header>
          <Progress total={100} value={voltage} indicating />
          <Header> Current {current}</Header>
          <Progress color="grey" inverted total={25} value={current} indicating />
        </>
      ) : (
        "No battery data received yet"
      )}

      {lastUpdated && <span>Last Updated: {lastUpdated.toLocaleTimeString()}</span>}
    </Segment>
  );
};

export default BatteryWidget;
