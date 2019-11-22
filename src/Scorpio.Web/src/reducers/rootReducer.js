import { combineReducers } from "redux";
import { connectRouter } from "connected-react-router";
import configReducer from "./configReducer";

const rootReducer = history =>
  combineReducers({
    router: connectRouter(history),
    configs: configReducer
  });

export default rootReducer;
