import { combineReducers } from "redux";
import { connectRouter } from "connected-react-router";
import configReducer from "./configReducer";
import sensorReducer from "./sensorReducer";
import streamsReducer from "./streamReducer";

const rootReducer = history =>
  combineReducers({
    router: connectRouter(history),
    configs: configReducer,
    sensors: sensorReducer,
    streams: streamsReducer
  });

export default rootReducer;
