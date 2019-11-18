import LogService from "../logService";
import GamepadProcessor from "./gamepadProcessor";
import { IXboxGamepadModel } from "./xboxGamepadModel";
import { isTSUndefinedKeyword } from "@babel/types";

const xboxGamepadId = "Xbox";

declare global {
  interface Window {
    scorpioGamepad: GamepadService;
  }
}

// TODO make this singleton
class GamepadService {
  constructor() {
    this.updateGamepadState = this.updateGamepadState.bind(this);
    window.scorpioGamepad = this;
  }

  private _connectedGamepadsIds: number[] = [];
  private _gamepadsState: IXboxGamepadModel[] = [];

  public init(): void {
    this.addEventListeners();
    this.doPoll();
  }

  public getGamepadState(index: number): IXboxGamepadModel | null {
    return this._gamepadsState.find(x => x.index === index) || null;
  }

  private doPoll(): void {
    window.requestAnimationFrame(this.updateGamepadState);
  }

  private updateGamepadState() {
    const gamepads = navigator.getGamepads();

    for (const gamepad of gamepads) {
      if (!gamepad || !gamepad.id.includes(xboxGamepadId)) continue;

      const scorpioPad = new GamepadProcessor();
      const newState = scorpioPad.updateState(gamepad);

      const currentState = this._gamepadsState.find(x => x.index === newState.index);
      if (currentState === undefined) {
        // new state scenario - push initial state
        this._gamepadsState.push(newState);
      } else {
        // already existing state scenario - just update
        const currentStateIndex = this._gamepadsState.indexOf(currentState);
        this._gamepadsState[currentStateIndex] = newState;
      }
    }

    this.doPoll();
  }

  private addEventListeners(): void {
    window.addEventListener("gamepadconnected", ((ev: GamepadEvent): void => {
      if (ev.gamepad.id.includes(xboxGamepadId)) {
        LogService.info(`Gamepad ${ev.gamepad.id} with index: ${ev.gamepad.index} connected`);
        this._connectedGamepadsIds.push(ev.gamepad.index);
      }
    }) as EventListener);

    window.addEventListener("gamepaddisconnected", ((ev: GamepadEvent): void => {
      LogService.info(`Gamepad ${ev.gamepad.id} with index: ${ev.gamepad.index} disconnected`);
      const index = this._connectedGamepadsIds.indexOf(ev.gamepad.index);
      this._connectedGamepadsIds.splice(index, 1);
    }) as EventListener);
  }
}

export default GamepadService;
