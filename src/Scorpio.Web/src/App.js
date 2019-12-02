import React from "react";
import { ConnectedRouter } from "connected-react-router";
import { Provider } from "react-redux";
import configureStore, { history, runSagas } from "./store/configureStore";
import initialState from "./store/initialState";
import MainComponent from "./MainComponent";
import "react-s-alert/dist/s-alert-default.css";
import "react-s-alert/dist/s-alert-css-effects/slide.css";

export const store = configureStore(initialState);
runSagas();

const App = () => {
  return (
    <Provider store={store}>
      <ConnectedRouter history={history}>
        <MainComponent />
      </ConnectedRouter>
    </Provider>
  );
};

export default App;
