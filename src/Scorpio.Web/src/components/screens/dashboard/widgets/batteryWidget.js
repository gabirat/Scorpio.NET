import React, { useEffect, useState } from "react";
import { TOPICS } from "../../../../constants/appConstants";
import MessagingService from "../../../../services/MessagingService";
import LogService from "../../../../services/LogService";
import { Progress, Segment, Header } from "semantic-ui-react";

const BatteryWidget = () => {
  const [voltage, setVoltage] = useState(0);
  const [current, setCurrent] = useState(0);

  const websocketHandler = data => {
    LogService.debug(`Battery widget received data`, data);

    try {
      const obj = JSON.parse(data);
      if (obj && obj.voltage) {
        const parsed = Number.parseFloat(obj.voltage);
        if (!isNaN(parsed)) setVoltage(parsed);
      }
      if (obj && obj.current) {
        const parsed = Number.parseFloat(obj.current);
        if (!isNaN(parsed)) setCurrent(parsed);
      }
    } catch {}
  };

  useEffect(() => {
    LogService.debug("xxxSubscribing: " + TOPICS.BATTERY_DATA);
    MessagingService.subscribe(TOPICS.BATTERY_DATA, websocketHandler);

    return () => {
      LogService.debug("__________Removing subscription: " + TOPICS.BATTERY_DATA);
      MessagingService.unsubscribe(TOPICS.BATTERY_DATA, websocketHandler);
    };
  });

  return (
    <Segment basic>
      <Header> Battery {voltage}</Header>
      <Progress total={100} value={voltage} indicating />
      <Header> Current {current}</Header>
      <Progress color="grey" inverted total={25} value={current} indicating />
    </Segment>
  );
};

export default BatteryWidget;
