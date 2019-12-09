import React from "react";
import { useSelector } from "react-redux";
import { withRouter } from "react-router-dom";
import SensorContainer from "./sensorContainer";

const ScienceChartsScreen = props => {
  const sensors = useSelector(x => x.sensors);

  const onEditClicked = (ev, sensorKey) => {
    ev.preventDefault();
    props.history.push(`/science/edit/sensor-data/${sensorKey}`);
  };

  return Array.isArray(sensors) && sensors.length > 0
    ? sensors.map(s => <SensorContainer sensor={s} key={s.name} onEditClicked={onEditClicked} />)
    : null;
};

export default withRouter(ScienceChartsScreen);
