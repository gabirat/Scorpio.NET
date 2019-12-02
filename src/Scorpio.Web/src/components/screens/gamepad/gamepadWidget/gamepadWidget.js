import React, { Component } from "react";
import GamepadStick from "./gamepadStick";
import GamepadTrigger from "./gamepadTrigger";
import GamepadService from "../../../../services/GamepadService";
import "./gamepadWidget.css";

export class GamepadWidget extends Component {
  constructor(props) {
    super(props);
    this._gamepadService = GamepadService;
    this.state = {
      gamepad: { index: -1, name: "", leftStick: { xVal: 0, yVal: 0 }, rightStick: { xVal: 0, yVal: 0 }, leftTrigger: 0, rightTrigger: 0 }
    };
  }

  componentDidMount() {
    const { refreshInterval } = this.props;
    this._interval = setInterval(this.doUpdate, refreshInterval ? refreshInterval : 50);
  }

  componentWillUnmount() {
    if (this._interval) clearInterval(this._interval);
  }

  doUpdate = () => {
    const { gamepadIndex, onUpdate } = this.props;
    const state = this._gamepadService.getGamepadState(gamepadIndex);
    if (state) {
      this.setState({ gamepad: state });
    }
    if (onUpdate && typeof onUpdate === "function") {
      onUpdate(state);
    }
  };

  render() {
    const { leftStick, rightStick, index, name, leftTrigger, rightTrigger } = this.state.gamepad;
    return (
      <>
        <div>
          Pad: {index} : {name}
        </div>
        <GamepadStick posX={leftStick.xVal} posY={leftStick.yVal} />
        <GamepadStick posX={rightStick.xVal} posY={rightStick.yVal} />
        <GamepadTrigger value={leftTrigger} />
        <GamepadTrigger value={rightTrigger} />
      </>
    );
  }
}
