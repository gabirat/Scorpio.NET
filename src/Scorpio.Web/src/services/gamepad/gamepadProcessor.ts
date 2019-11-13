import { IXboxGamepadModel } from "./xboxGamepadModel";

class GamepadProcessor {
  // do all the mixing/scaling/validating/filtering logic here
  public updateState(state: Gamepad): IXboxGamepadModel {
    return {
      index: state.index,
      name: state.id,
      leftTrigger: state.buttons[6] && state.buttons[6].value,
      rightTrigger: state.buttons[7] && state.buttons[7].value,
      leftStick: {
        xVal: state.axes[0],
        yVal: state.axes[1]
      },
      rightStick: {
        xVal: state.axes[2],
        yVal: state.axes[3]
      }
    };
  }
}

export default GamepadProcessor;
