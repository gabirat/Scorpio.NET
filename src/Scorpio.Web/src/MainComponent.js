import React, { Component } from "react";
import { connect } from "react-redux";
import { bindActionCreators } from "redux";
import { withRouter } from "react-router-dom";
import * as actions from "./actions";
import { Route, Switch, Redirect } from "react-router-dom";
import NotFound from "./components/common/notFound";
import NavBar from "./components/navbar/navbar";
import Alert from "react-s-alert";
import { genericApi } from "./api/genericApi";
import { API } from "./constants/appConstants";
import AlertDispatcher from "./services/AlertDispatcher";
import LogService from "./services/LogService";
import dashboardScreen from "./components/screens/dashboard/dashboardScreen";
import streamScreen from "./components/screens/stream/streamScreen";
import gamepadScreen from "./components/screens/gamepad/gamepadScreen";
import aboutScreen from "./components/screens/about/aboutScreen";
import sensorsScreen from "./components/screens/sensors/sensorsScreen";

import GamepadService from "./services/GamepadService";

class MainComponent extends Component {
  constructor(props) {
    super(props);
    this.initGamepad();
  }

  initGamepad() {
    window.scorpioGamepad = GamepadService;
    GamepadService.init();
  }

  async componentDidMount() {
    const result = await genericApi(API.CONFIG.GET_ALL, "GET");
    if (result && result.response && result.response.ok && result.body && Array.isArray(result.body)) {
      const configs = result.body || [];
      LogService.debug("Fetched configs", configs);
      this.props.actions.setConfigs(configs);
    } else {
      AlertDispatcher.dispatch({ type: "error", text: "Could not fetch configs - check if API is running" });
    }
  }

  render() {
    return (
      <>
        <NavBar className="fullWidth">
          <Switch>
            <Route exact path="/" render={_ => <Redirect to={"/dashboard"} />} />
            <Route exact path="/dashboard" component={dashboardScreen} />
            <Route exact path="/stream" component={streamScreen} />
            <Route exact path="/gamepad" component={gamepadScreen} />
            <Route exact path="/about" component={aboutScreen} />
            <Route exact path="/sensors" component={sensorsScreen} />
            <Route exact path="/not-found" component={NotFound} />
            <Redirect to="/not-found" />
          </Switch>
        </NavBar>
        <Alert stack={{ limit: 4 }} beep timeout={5000} />
      </>
    );
  }
}

function mapStateToProps(state) {
  return { state };
}

function mapDispatchToProps(dispatch) {
  return {
    actions: bindActionCreators(actions, dispatch)
  };
}

export default withRouter(connect(mapStateToProps, mapDispatchToProps)(MainComponent));
