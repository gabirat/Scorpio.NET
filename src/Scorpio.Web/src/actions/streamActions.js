import * as types from "./actionTypes";

export function setStreams(streams) {
  return {
    type: types.SET_STREAMS,
    payload: streams
  };
}
