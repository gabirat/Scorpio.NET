import React, { Component } from "react";
import { Grid, Header, Image } from "semantic-ui-react";

class AboutScreen extends Component {
  render() {
    const swaggerUrl = process.env.REACT_APP_BACKEND_URL + "/swagger";
    return (
      <div style={{ marginTop: "150px" }}>
        <Grid textAlign="center" style={{ height: "100%", maxWidth: "500px", margin: "auto" }} verticalAlign="middle">
          <Grid.Row>
            <Header as="h1">Scorpio rover control App</Header>
          </Grid.Row>
          <Grid.Row>
            <Header>Created by: Mateusz Kryszczak</Header>
          </Grid.Row>
          <Grid.Row>
            <a href={swaggerUrl} rel="noopener noreferrer" target="_blank">
              <Image size="medium" src={process.env.PUBLIC_URL + "/swagger.png"} />
            </a>
          </Grid.Row>
        </Grid>
      </div>
    );
  }
}

export default AboutScreen;
