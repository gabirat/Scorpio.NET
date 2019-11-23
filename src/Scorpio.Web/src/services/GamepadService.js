import LogService from "./LogService";

const xboxGamepadId = "Xbox";

class GamepadService {
  constructor() {
    this._connectedGamepadsIds = [];
    this._gamepadsState = [];

    this._updateGamepadState = this._updateGamepadState.bind(this);
  }

  init() {
    this._addEventListeners();
    this._doPoll();
  }

  getGamepadsState() {
    return this._gamepadsState || null;
  }

  getGamepadState(index) {
    if (this._gamepadsState && this._gamepadsState.length > 0) {
      const indexNum = Number.parseInt(index);
      return this._gamepadsState.find(x => x.index === indexNum) || null;
    }
    return null;
  }

  _doPoll() {
    window.requestAnimationFrame(this._updateGamepadState);
  }

  _updateGamepadState() {
    const gamepads = navigator.getGamepads();

    for (const gamepad of gamepads) {
      if (!gamepad || !gamepad.id.includes(xboxGamepadId)) continue;
      const currentState = this._gamepadsState.find(x => x.index === gamepad.index);
      if (currentState === undefined) {
        // new state scenario - push initial state
        const mapped = this._map(gamepad);
        this._gamepadsState.push(mapped);
      } else {
        // already existing state scenario - just update
        const mapped = this._map(gamepad);
        const currentStateIndex = this._gamepadsState.indexOf(currentState);
        this._gamepadsState[currentStateIndex] = mapped;
      }
    }
    this._doPoll();
  }

  _map(raw) {
    if (!raw) return {};
    return {
      index: raw.index,
      name: raw.id,
      leftTrigger: raw.buttons[6] && raw.buttons[6].value,
      rightTrigger: raw.buttons[7] && raw.buttons[7].value,
      leftStick: {
        xVal: raw.axes[0],
        yVal: raw.axes[1]
      },
      rightStick: {
        xVal: raw.axes[2],
        yVal: raw.axes[3]
      }
    };
  }

  _addEventListeners() {
    window.addEventListener("gamepadconnected", ev => {
      if (ev.gamepad.id.includes(xboxGamepadId)) {
        LogService.info(`Gamepad ${ev.gamepad.id} with index: ${ev.gamepad.index} connected`);
        this._connectedGamepadsIds.push(ev.gamepad.index);
      }
    });

    window.addEventListener("gamepaddisconnected", ev => {
      LogService.info(`Gamepad ${ev.gamepad.id} with index: ${ev.gamepad.index} disconnected`);
      const index = this._connectedGamepadsIds.indexOf(ev.gamepad.index);
      this._connectedGamepadsIds.splice(index, 1);
    });
  }
}

const singleton = new GamepadService();
export default singleton;
