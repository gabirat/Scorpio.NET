import * as am4charts from "@amcharts/amcharts4/charts";
import * as am4core from "@amcharts/amcharts4/core";
import am4themes_animated from "@amcharts/amcharts4/themes/animated";
import _ from "lodash";
import PropTypes from "prop-types";
import React from "react";
import { Header, Segment } from "semantic-ui-react";
import { genericApi } from "../../../../api/genericApi";
import { API } from "../../../../constants/appConstants";
import WidgetErrorFallback from "../../../common/widgetErrorFallback";
import Spinner from "../../../common/spinner";

export default class SingleChart extends React.PureComponent {
  constructor(props) {
    super(props);
    this.state = { chartData: [], physicalProperty: {}, isFetched: false, hasError: false };
    this.mountingId = _.uniqueId("canv_");
    this._isMounted = false;
  }

  async componentDidMount() {
    this._isMounted = true;
    await this.fetchMapData(() => {
      this.prepareChart();
    });
  }

  componentWillUnmount() {
    this._isMounted = false;
  }

  fetchMapData = async callback => {
    const { nodeId, magnitude } = this.props;

    // TODO maybe move to redux to improve performance
    try {
      const resp = await genericApi(`${API.NODE_DATA.GET_BY_ID}${nodeId}?magnitude=${magnitude}`, "GET");
      const { body } = resp;

      if (!body || !body.data || !body.data.values || !body.data.values.length === 0 || !body.data.physicalProperty) {
        throw new Error("API error");
      }

      let dataObj = body.data.values.map(x => {
        return {
          timeStamp: new Date(x.timeStamp),
          magnitude: x.value
        };
      });

      if (this._isMounted) this.setState({ chartData: dataObj, physicalProperty: body.data.physicalProperty, isFetched: true }, callback);
    } catch (err) {
      this.setState({ hasError: true });
    }
  };

  prepareChart = () => {
    am4core.useTheme(am4themes_animated);
    let chart = am4core.create(this.mountingId, am4charts.XYChart);
    chart.paddingRight = 15;
    chart.data = this.state.chartData;

    let dateAxis = chart.xAxes.push(new am4charts.DateAxis());
    dateAxis.tooltipDateFormat = "HH:mm, d MMMM";
    dateAxis.baseInterval = {
      timeUnit: "second",
      count: 1
    };

    // X axis
    let valueAxis = chart.yAxes.push(new am4charts.ValueAxis());
    valueAxis.tooltip.disabled = false;

    // Cursor
    chart.cursor = new am4charts.XYCursor();
    chart.cursor.lineY.opacity = 0;

    // Y axis
    let series = chart.series.push(new am4charts.LineSeries()); //new am4charts.ColumnSeries()
    series.dataFields.dateX = "timeStamp";
    series.dataFields.valueY = "magnitude";
    series.tooltipText = "Value: [bold]{valueY}[/]";
    series.fillOpacity = 0.3;

    // Scrollbar
    chart.scrollbarX = new am4charts.XYChartScrollbar();
    chart.scrollbarX.series.push(series);

    chart.events.on("datavalidated", function() {
      dateAxis.zoom({ start: 0.8, end: 1 });
    });
  };

  render() {
    const { hasError, isFetched, physicalProperty } = this.state;
    const { widgetTitle } = this.props;

    return (
      <>
        {hasError ? (
          <WidgetErrorFallback />
        ) : (
          <Segment className="fullHeight">
            {isFetched ? (
              <>
                <Header textAlign="center" as="h5">
                  {widgetTitle} - {physicalProperty.name} {physicalProperty.unit}
                </Header>
                <div id={this.mountingId} className="fullHeight" />
              </>
            ) : (
              <Spinner />
            )}
          </Segment>
        )}
      </>
    );
  }
}

SingleChart.propTypes = {
  nodeId: PropTypes.number.isRequired,
  magnitude: PropTypes.string.isRequired
};
