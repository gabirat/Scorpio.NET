import React, { Component } from "react";
import { Route } from "react-router-dom";
import { bindActionCreators, Dispatch } from "redux";
import { connect } from "react-redux";
import { AppState } from "./redux/reducers/rootReducer";
import { fetchConfigs, FetchConfigsAction } from "./redux/actions/configActions";

import VideoStream from "./components/dashboardPage/widgets/VideoStream/VideoStream";
import ScorpioNavbar from "./components/NavBar/ScorpioNavbar";
import DashboardPage from "./components/dashboardPage/dashboardPage";
import GamepadPage from "./components/gamepadPage/gamepadPage";

import GamepadService from "./services/gamepad/gamepadService";

declare global {
  interface Window {
    scorpioGamepad: GamepadService | {};
  }
}

class MainComponent extends Component {
  private _gamepadService: GamepadService;
  constructor(props: any) {
    super(props);
    window.scorpioGamepad = {};
    this._gamepadService = GamepadService.getInstance();
    this._gamepadService.init();
    props.fetchConfigs();
  }

  render() {
    console.log(this.props);
    return (
      <>
        <ScorpioNavbar />
        <Route exact path="/" component={DashboardPage} />
        <Route exact path="/dashboard" component={DashboardPage} />
        <Route exact path="/gamepad" component={GamepadPage} />
        <Route path="/stream" component={VideoStream} />
      </>
    );
  }
}

const mapStateToProps = (state: AppState) => ({
  config: state.config
});

const mapDispatchToProps = (dispatch: Dispatch<FetchConfigsAction>) =>
  bindActionCreators(
    {
      fetchConfigs: fetchConfigs
    },
    dispatch
  );

export default connect(mapStateToProps, mapDispatchToProps)(MainComponent);
