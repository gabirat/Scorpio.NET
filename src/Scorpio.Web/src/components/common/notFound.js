import React, { PureComponent } from "react";
import { Link } from "react-router-dom";
import { Grid, Header, Image } from "semantic-ui-react";

export default class NotFound extends PureComponent {
  render() {
    return (
      <div style={{ marginTop: "150px" }}>
        <Grid textAlign="center" style={{ height: "100%", maxWidth: "500px", margin: "auto" }} verticalAlign="middle">
          <Grid.Column width="8">
            <Image size="small" src={process.env.PUBLIC_URL + "/martian.png"} />
          </Grid.Column>
          <Grid.Column width="8">
            <Header as="h2" color="teal" textAlign="center">
              Not found
            </Header>
            <Link to="/">Go to dashboard</Link>
          </Grid.Column>
        </Grid>
      </div>
    );
  }
}
