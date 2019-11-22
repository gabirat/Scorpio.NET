import React, { Component } from "react";
import NavBar from "./NavBar";

const leftItems = [
  { as: "a", content: "Dashboard", key: "dashboard", url: "dashboard" },
  { as: "a", content: "Gamepad", key: "gamepad", url: "gamepad" },
  { as: "a", content: "Stream", key: "stream", url: "stream" }
];

const rightItems = [{ as: "a", content: "NotHidingTab1", key: "nothidingtab1", url: "nothidingtab1" }];

class ScorpioNavbar extends Component<{}, {}> {
  render() {
    return <NavBar leftItems={leftItems} rightItems={rightItems} />;
  }
}

export default ScorpioNavbar;
