import React, { Component } from "react";
import { BrowserRouter as Router } from "react-router-dom";
import { Provider } from "react-redux";
import store from "./redux/store";
import MainComponent from "./MainComponent";
import "./App.css";
import "semantic-ui-css/semantic.min.css";

class App extends Component {
  render() {
    return (
      <Provider store={store}>
        <Router>
          <MainComponent />
        </Router>
      </Provider>
    );
  }
}

export default App;
