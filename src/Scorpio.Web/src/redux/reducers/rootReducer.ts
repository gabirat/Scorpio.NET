import { combineReducers } from "redux";
import { configReducer } from "./configReducer";

export const rootReducer = combineReducers({
  config: configReducer
});

export type AppState = ReturnType<typeof rootReducer>;
