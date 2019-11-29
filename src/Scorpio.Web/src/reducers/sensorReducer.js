import * as types from "../actions/actionTypes";
import initialState from "../store/initialState";

export default function sensorReducer(state = initialState.sensors, action) {
  switch (action.type) {
    case types.SET_SENSORS:
      return action.payload;

    default:
      return state;
  }
}
