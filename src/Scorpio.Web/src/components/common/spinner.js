import React, { PureComponent } from "react";
import { RiseLoader } from "halogenium";
import "./common.css";

export default class Spinner extends PureComponent {
  render() {
    const { fade } = this.props;
    return (
      <div className={`spinner-overlay ${fade ? " spinner-overlay-fade" : ""}`}>
        <RiseLoader className="spinner-overlay-content" color="#26A65B" size="16px" margin="4px" />
      </div>
    );
  }
}
