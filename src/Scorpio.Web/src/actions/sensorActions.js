import * as types from "../actions/actionTypes";

export function setSensors(sensors) {
  return {
    type: types.SET_SENSORS,
    payload: sensors
  };
}
