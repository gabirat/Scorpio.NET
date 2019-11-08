import React, { Component } from "react";
import Gamepad from "react-gamepad";

class GamepadPoller extends Component {
  connectHandler = (gamepadIndex: number): void => {
    console.log(`Gamepad ${gamepadIndex} connected !`);
  };

  disconnectHandler = (gamepadIndex: number): void => {
    console.log(`Gamepad ${gamepadIndex} connected !`);
  };

  onButtonChangeHandler = (buttonName: any, down: any): void => {
    console.log(buttonName, down);
  };

  onAxisChangeHandler = (axisName: any, value: any): void => {
    console.log(axisName, value);
  };

  render() {
    return (
      <Gamepad
        onConnect={this.connectHandler}
        onDisconnect={this.disconnectHandler}
        onButtonChange={this.onButtonChangeHandler}
        onAxisChange={this.onAxisChangeHandler}
      >
        <span></span>
      </Gamepad>
    );
  }
}

export default GamepadPoller;
