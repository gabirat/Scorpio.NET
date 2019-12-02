import React, { PureComponent } from "react";
import { Segment } from "semantic-ui-react";

export default class WidgetErrorFallback extends PureComponent {
  render() {
    return (
      <Segment textAlign="center" className="fullHeight flex-center">
        Oops! There is something wrong with configuration.
      </Segment>
    );
  }
}
