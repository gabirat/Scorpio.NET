import React, { useEffect, useState } from "react";
import { Segment, Icon } from "semantic-ui-react";

const Statusoverlay = () => {
  const [blink, setBlink] = useState(false);

  useEffect(() => {
    const interval = setInterval(() => {
      setBlink(blnk => !blnk);
    }, 1000);

    return () => {
      clearInterval(interval);
    };
  }, []);

  return (
    <Segment className="scorpio-status-overlay-container">
      <div>
        <Icon className={blink ? "scorpio-heartbeat-blink" : "scorpio-heartbeat-normal"} color="red" name="heartbeat"></Icon>API: connected
      </div>
      <div>
        <Icon className={blink ? "scorpio-heartbeat-blink" : "scorpio-heartbeat-normal"} name="wheelchair"></Icon> Lazik: rak
      </div>
    </Segment>
  );
};

export default Statusoverlay;
