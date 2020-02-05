import React, { Component } from "react";
import { Button, Dropdown, Segment } from "semantic-ui-react";
import MessagingService from "../../../services/MessagingService";
import { TOPICS } from "../../../constants/appConstants";

class ConsoleScreen extends Component {
  constructor(props) {
    super(props);
    this.handlers = {};
    this.state = { logs: [], rows: 25 };
  }

  componentDidMount() {
    Object.values(TOPICS).forEach(topic => {
      this.handlers[topic] = data => this.writeLog(data);
      MessagingService.subscribe(topic, this.handlers[topic]);
    });
  }

  componentWillUnmount() {
    Object.values(TOPICS).forEach(topic => {
      MessagingService.unsubscribe(topic, this.handlers[topic]);
    });
  }

  clearLog = () => this.setState({ logs: [] });

  writeLog = eventName =>
    this.setState(prevState => ({
      logs: [`${new Date().toLocaleTimeString()}: ${eventName}`, ...prevState.logs].slice(0, prevState.rows)
    }));

  render() {
    const { logs, rows } = this.state;

    return (
      <Segment>
        <Button color="google plus" onClick={this.clearLog}>
          Clear
        </Button>
        <span style={{ marginLeft: "2rem" }}>
          Rows
          <Dropdown
            style={{ marginLeft: "1rem" }}
            onChange={(_, d) => this.setState({ rows: d.value })}
            additionLabel="add"
            selection
            compact
            defaultValue={rows}
            options={[
              { key: 1, value: 1, text: 1 },
              { key: 10, value: 10, text: 10 },
              { key: 25, value: 25, text: 25 },
              { key: 50, value: 50, text: 50 },
              { key: 75, value: 75, text: 75 },
              { key: 100, value: 100, text: 100 }
            ]}
          />
        </span>
        <Segment secondary>
          <pre>
            {logs.map((e, i) => (
              <div style={{ marginTop: "5px", padding: "5px", borderRadius: "10px", backgroundColor: "#9d9e9e" }} key={i}>
                {e}
              </div>
            ))}
          </pre>
        </Segment>
      </Segment>
    );
  }
}

export default ConsoleScreen;
