import React, { Component } from "react";
import WidgetErrorFallback from "./widgetErrorFallback";
import LogService from "../../services/LogService";

export default class WidgetErrorBoundary extends Component {
  constructor(props) {
    super(props);
    this.state = { errorOccurred: false };
  }

  componentDidCatch(error, info) {
    this.setState({ errorOccurred: true });
    LogService.error("Widget error: ", error, info);
  }

  render() {
    if (this.state.errorOccurred) {
      return <WidgetErrorFallback />;
    }
    return this.props.children;
  }
}
