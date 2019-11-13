import React, { Component } from "react";
import GamepadStick from "./gamepadStick";
import GamepadTrigger from "./gamepadTrigger";
import { IXboxGamepadModel } from "../../../../services/gamepad/xboxGamepadModel";
import "./rawGamepadWidget.css";

interface IRawGamepadWidgetProps {
  gamepadIndex: number;
  refreshInterval?: number;
}

interface IRawGamepadWidgetState {
  gamepad: IXboxGamepadModel;
}

export class RawGamepadWidget extends Component<IRawGamepadWidgetProps, IRawGamepadWidgetState> {
  private _interval: NodeJS.Timeout | null = null;

  constructor(props: IRawGamepadWidgetProps) {
    super(props);
    this.state = {
      gamepad: { index: 0, name: "", leftStick: { xVal: 0, yVal: 0 }, rightStick: { xVal: 0, yVal: 0 }, leftTrigger: 0, rightTrigger: 0 }
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
    const { gamepadIndex } = this.props;
    const gamepad = window.scorpioGamepad.getGamepadState(gamepadIndex);
    if (gamepad) {
      this.setState({ gamepad });
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
