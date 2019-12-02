import React, { Component } from "react";
import PropTypes from "prop-types";
import { Statistic, Segment } from "semantic-ui-react";
import { genericApi } from "../../../../api/genericApi";
import Spinner from "../../../common/spinner";
import { API } from "../../../../constants/appConstants";
import moment from "moment";
import WidgetErrorFallback from "../../../common/widgetErrorFallback";

export default class NumericStatistic extends Component {
  constructor(props) {
    super(props);
    this.state = { isFetched: false, value: null, physicalProperty: { id: null }, hasError: false };
    this._isMounted = false;
  }

  async componentDidMount() {
    this._isMounted = true;
    await this.fetchMapData();
  }

  componentWillUnmount() {
    this._isMounted = false;
  }

  fetchMapData = async () => {
    // TODO redux
    const { magnitude, nodeId } = this.props;
    try {
      const resp = await genericApi(`${API.NODE_DATA.GET_BY_ID}${nodeId}?magnitude=${magnitude}&limit=1`, "GET");
      const { body } = resp;

      if (!body || !body.data || !body.data.values || !body.data.values.length === 0 || !body.data.physicalProperty) {
        throw new Error("API error");
      }

      if (this._isMounted) {
        this.setState({
          isFetched: true,
          physicalProperty: body.data.physicalProperty,
          value: {
            t: body.data.values[0].timeStamp,
            mag: body.data.values[0].value
          }
        });
      }
    } catch (err) {
      console.error(err);
      this.setState({ hasError: true });
    }
  };

  render() {
    const { hasError, isFetched, value, physicalProperty } = this.state;
    const { widgetTitle } = this.props;
    return (
      <>
        {hasError ? (
          <WidgetErrorFallback />
        ) : (
          <Segment textAlign="center" className="fullHeight flex-center">
            {isFetched ? (
              <Statistic>
                <Statistic.Value text>
                  {widgetTitle}
                  <br />
                  {`${value.mag} ${physicalProperty.unit}`}
                </Statistic.Value>
                <Statistic.Label>{moment(value.t).format("MMMM Do YYYY, h:mm:ss a")}</Statistic.Label>
              </Statistic>
            ) : (
              <Spinner />
            )}
          </Segment>
        )}
      </>
    );
  }
}

NumericStatistic.propTypes = {
  nodeId: PropTypes.number.isRequired,
  magnitude: PropTypes.string.isRequired
};
