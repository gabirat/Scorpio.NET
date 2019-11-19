import { createStore, applyMiddleware } from "redux";
import thunk from "redux-thunk";
import { rootReducer } from "./reducers/rootReducer";
//import logger from "redux-logger";

// todo middlewares
export default createStore(rootReducer, applyMiddleware(thunk));
