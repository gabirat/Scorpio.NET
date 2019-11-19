import React, { Component } from "react";
import { Route } from "react-router-dom";
import { bindActionCreators, Dispatch } from "redux";
import { connect } from "react-redux";
import { AppState } from "./redux/reducers/rootReducer";
import { fetchConfigs, FetchConfigsAction } from "./redux/actions/configActions";

import VideoStream from "./components/dashboard/widgets/VideoStream/VideoStream";
import ScorpioNavbar from "./components/NavBar/ScorpioNavbar";
import DashboardPage from "./components/dashboard/dashboardPage";

class MainComponent extends Component {
  constructor(props: any) {
    super(props);
    props.fetchConfigs();
  }

  render() {
    console.log(this.props);
    return (
      <>
        <ScorpioNavbar />
        <Route exact path="/" component={DashboardPage} />
        <Route exact path="/dashboard" component={DashboardPage} />
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
