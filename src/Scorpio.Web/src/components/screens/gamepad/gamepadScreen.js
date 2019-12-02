import React, { Component, useState } from "react";
import PropTypes from "prop-types";
import { connect } from "react-redux";
import { bindActionCreators } from "redux";
import { withRouter } from "react-router-dom";
import * as actions from "../../../actions";
import { GamepadWidget } from "./gamepadWidget/gamepadWidget";
import { Message, Segment, Accordion, Icon } from "semantic-ui-react";
import GamepadService from "../../../services/GamepadService";

const xboxGamepadId = "Xbox";

class GamepadScreen extends Component {
  constructor(props) {
    super(props);
    this.state = { gamepads: {} }; // gamepad object is of shape: id: name
    this._gamepadService = GamepadService;
  }

  componentDidMount() {
    this.initPadPoller();
  }

  initPadPoller = () => {
    const gamepads = this._gamepadService.getGamepadsState();
    // We already have connected gamepads in service -> just map them to render
    if (gamepads && gamepads.length > 0) {
      const reducedGamepads = gamepads.reduce((accu, curr) => {
        accu[curr.index] = curr.id;
        return accu;
      }, {});
      this.setState({ gamepads: reducedGamepads });
      // No pads so far - subscribe to changes and map when new one is connected
    } else {
      this.subscribe();
    }
  };

  subscribe = () => {
    window.addEventListener("gamepadconnected", ev => {
      if (ev.gamepad.id.includes(xboxGamepadId)) {
        this.setState(prevState => ({
          gamepads: {
            ...prevState.gamepads,
            [ev.gamepad.index]: ev.gamepad.id
          }
        }));
      }
    });
  };

  render() {
    const { gamepads } = this.state;
    const hasGamepads = gamepads && Object.keys(gamepads).length > 0;

    return (
      <div style={{ margin: "1rem" }}>
        {hasGamepads ? (
          <>
            {Object.keys(gamepads).map(key => {
              return <SingleGamepadPanel key={key} gamepadIndex={key} />;
            })}
          </>
        ) : (
          <Message color="yellow" header="No gamepads found" list={["Connect gamepad either move any stick/press button"]} />
        )}
      </div>
    );
  }
}

const SingleGamepadPanel = ({ gamepadIndex }) => {
  const [rawState, setRawState] = useState(null);
  const [isRawStateExpanded, setRawExpanded] = useState(false);

  const stateJson = rawState ? JSON.stringify(rawState, null, 1) : "No data";

  return (
    <Segment key={gamepadIndex} textAlign="center">
      <GamepadWidget key={gamepadIndex} gamepadIndex={gamepadIndex} refreshInterval={40} onUpdate={x => setRawState(x)} />
      <Accordion style={{ paddingTop: "1rem" }}>
        <Accordion.Title active={isRawStateExpanded} onClick={() => setRawExpanded(!isRawStateExpanded)}>
          <Icon name="dropdown" />
          Show raw state - JSON
        </Accordion.Title>
        <Accordion.Content active={isRawStateExpanded}>
          <pre style={{ backgroundColor: "lightgray" }}>{stateJson}</pre>
        </Accordion.Content>
      </Accordion>
    </Segment>
  );
};

function mapStateToProps(state) {
  return { state };
}

function mapDispatchToProps(dispatch) {
  return {
    actions: bindActionCreators(actions, dispatch)
  };
}

SingleGamepadPanel.propTypes = {
  gamepadIndex: PropTypes.number.isRequired
};

export default withRouter(connect(mapStateToProps, mapDispatchToProps)(GamepadScreen));
