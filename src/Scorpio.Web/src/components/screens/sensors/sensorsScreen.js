import React, { Component } from "react";
import { connect } from "react-redux";
import { bindActionCreators } from "redux";
import { withRouter } from "react-router-dom";
import * as actions from "../../../actions";
import { genericApi } from "../../../api/genericApi";
import { API } from "../../../constants/appConstants";

class SensorsScreen extends Component {
  constructor(props) {
    super(props);
    this.state = { sensors: [] };
  }

  async componentDidMount() {
    const result = await genericApi(API.SENSORS.GET_ALL, "GET");
    if (result.response.ok) {
      this.setState({ sensors: result.body });
    }
  }

  render() {
    console.log(this.state.sensors);
    return <div>sensors</div>;
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

export default withRouter(connect(mapStateToProps, mapDispatchToProps)(SensorsScreen));
