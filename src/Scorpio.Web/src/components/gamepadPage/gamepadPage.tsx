import React, { Component } from "react";
import RawGamepadWidget from "../dashboardPage/widgets/rawGamepadWidget";

class GamepadPage extends Component {
  render() {
    return (
      <div>
        <RawGamepadWidget gamepadIndex={0} />
      </div>
    );
  }
}

export default GamepadPage;
