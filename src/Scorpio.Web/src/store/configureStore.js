import { createStore, compose, applyMiddleware } from "redux";
import reduxImmutableStateInvariant from "redux-immutable-state-invariant";
import thunk from "redux-thunk";
import * as historyBase from "history";
import { connectRouter, routerMiddleware } from "connected-react-router";
import rootReducer from "../reducers/rootReducer";
//import logger from "redux-logger";
import createSagaMiddleware from "redux-saga";
import { deleteConfigSaga } from "../sagas/deleteConfigSaga";
import { addUpdateConfigSaga } from "../sagas/addUpdateConfigSaga";

export const history = historyBase.createBrowserHistory();
window.reactHistory = history;
const connectRouterHistory = connectRouter(history);
const sagaMiddleware = createSagaMiddleware();

function configureStoreProd(initialState) {
  const reactRouterMiddleware = routerMiddleware(history);
  const middlewares = [thunk, reactRouterMiddleware, sagaMiddleware];

  return createStore(connectRouterHistory(rootReducer(history)), initialState, compose(applyMiddleware(...middlewares)));
}

function configureStoreDev(initialState) {
  const reactRouterMiddleware = routerMiddleware(history);
  const middlewares = [reduxImmutableStateInvariant(), thunk, reactRouterMiddleware, sagaMiddleware]; //logger

  const composeEnhancers = window.__REDUX_DEVTOOLS_EXTENSION_COMPOSE__ || compose; // add support for Redux dev tools
  return createStore(connectRouterHistory(rootReducer(history)), initialState, composeEnhancers(applyMiddleware(...middlewares)));
}

const configureStore = process.env.NODE_ENV === "production" ? configureStoreProd : configureStoreDev;

export function runSagas() {
  sagaMiddleware.run(deleteConfigSaga);
  sagaMiddleware.run(addUpdateConfigSaga);
}

export default configureStore;
