import * as types from "../actions/actionTypes";
import initialState from "../store/initialState";

export default function streamReducer(state = initialState.streams, action) {
  switch (action.type) {
    case types.SET_STREAMS:
      return action.payload;

    default:
      return state;
  }
}
