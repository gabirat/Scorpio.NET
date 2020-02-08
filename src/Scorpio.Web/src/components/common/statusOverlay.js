import React, { useState } from "react";
import { Segment, Icon } from "semantic-ui-react";
import AppHealth from "./appHealth";

const Statusoverlay = () => {
  const [isExpanded, setExpanded] = useState(false);

  return (
    <Segment className="scorpio-status-overlay-container">
      <div className={`${isExpanded ? "status-wrapper-expanded" : ""}`}>
        <div onClick={_ => setExpanded(!isExpanded)} className="flex fullWidth pointer">
          <div>{"Health & Control"}</div>
          <Icon className="mrg-left-auto mrg-right-sm" name="expand arrows alternate" />
        </div>
      </div>
      <div className={`${isExpanded ? "visible" : "display-none"}`}>
        <AppHealth />
      </div>
    </Segment>
  );
};

export default Statusoverlay;
