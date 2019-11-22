import React, { Component } from "react";
import { Select, DropdownItemProps } from "semantic-ui-react";
import GamepadStick from "./gamepadStick";
import GamepadTrigger from "./gamepadTrigger";
import { IXboxGamepadModel } from "../../../../services/gamepad/xboxGamepadModel";
import MessagingService from "../../../../services/messagingService";
import GamepadService from "../../../../services/gamepad/gamepadService";
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
  private _messagingService: MessagingService;

  constructor(props: IRawGamepadWidgetProps) {
    super(props);
    this._messagingService = MessagingService.getInstance();
    this._messagingService.connect();
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

  doUpdate = (): void => {
    const { gamepadIndex } = this.props;
    const windowScorpioGamepad = window.scorpioGamepad as GamepadService;

    if (windowScorpioGamepad && typeof windowScorpioGamepad.getGamepadState === "function") {
      const gamepad = windowScorpioGamepad.getGamepadState(gamepadIndex);
      if (gamepad) {
        this._messagingService.send("data", gamepad);
        console.log("sending", gamepad);
        this.setState({ gamepad });
      }
    }
  };

  getDropdownOptions = (): DropdownItemProps[] => {
    const windowScorpioGamepad = window.scorpioGamepad as GamepadService;
    if (windowScorpioGamepad && typeof windowScorpioGamepad.getConnectedGamepads === "function") {
      const options = windowScorpioGamepad.getConnectedGamepads();
      console.warn(options);
    }
    return [{ key: "asd", value: "asd", text: "asdas12312d" }];
  };

  render() {
    const { leftStick, rightStick, index, name, leftTrigger, rightTrigger } = this.state.gamepad;
    return (
      <>
        <Select
          placeholder="Select gamepad"
          options={this.getDropdownOptions()}
          onChange={(ev, selection) => console.log(selection.value)}
        />
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
