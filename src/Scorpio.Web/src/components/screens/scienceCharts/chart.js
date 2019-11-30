import React, { useEffect } from "react";
import PropTypes from "prop-types";
import * as am4charts from "@amcharts/amcharts4/charts";
import * as am4core from "@amcharts/amcharts4/core";
import theme_frozen from "@amcharts/amcharts4/themes/frozen";
import theme_animated from "@amcharts/amcharts4/themes/animated";

const Chart = ({ sensor, data }) => {
  let chart = null;

  useEffect(() => {
    prepareChart();

    return () => {
      if (chart) {
        chart.dispose();
      }
    };
  }, [data, sensor]);

  const prepareChart = () => {
    am4core.useTheme(theme_frozen);
    am4core.useTheme(theme_animated);
    chart = am4core.create(`chart-container-${sensor.sensorKey}`, am4charts.XYChart);
    chart.paddingRight = 15;

    const mappedData = Array.isArray(data)
      ? data.map(d => {
          return {
            id: d.id,
            key: sensor.sensorKey,
            t: new Date(d.timeStamp),
            v: d.value
          };
        })
      : [];

    chart.data = mappedData;

    let dateAxis = chart.xAxes.push(new am4charts.DateAxis());
    dateAxis.tooltipDateFormat = "HH:mm";
    dateAxis.baseInterval = {
      timeUnit: "second",
      count: 1
    };

    // Y axis
    let valueAxis = chart.yAxes.push(new am4charts.ValueAxis());

    // Y series
    let series = chart.series.push(new am4charts.LineSeries());
    series.dataFields.dateX = "t";
    series.dataFields.valueY = "v";
    series.fillOpacity = 0.3;
    series.strokeWidth = 1.5;
    series.minBulletDistance = 10;
    series.tooltip.label.interactionsEnabled = true;
    series.tooltip.pointerOrientation = "vertical";
    series.tooltipHTML = `<center><strong>{valueY}</strong></center>
    <hr />
    <center><input type="button" value="Edit" onclick="window.reactHistory.push('/science/edit/sensor-data/{key}/{id}')" /></center>`;

    series.tooltip.pointerOrientation = "vertical";
    series.tooltip.background.fillOpacity = 0.5;
    series.tooltip.label.padding(12, 12, 12, 12);

    /* Create a cursor */
    chart.cursor = new am4charts.XYCursor();
    chart.cursor.xAxis = dateAxis;
    chart.cursor.snapToSeries = series;

    // Scrollbar
    chart.scrollbarX = new am4charts.XYChartScrollbar();
    chart.scrollbarX.series.push(series);

    chart.events.on("datavalidated", function() {
      dateAxis.zoom({ start: 0, end: 1 });
    });
  };

  return <div id={`chart-container-${sensor.sensorKey}`} style={{ width: "100%", height: "100%" }}></div>;
};

Chart.propTypes = {
  sensor: PropTypes.shape({
    sensorKey: PropTypes.string.isRequired
  }),
  data: PropTypes.array.isRequired
};

export default Chart;
