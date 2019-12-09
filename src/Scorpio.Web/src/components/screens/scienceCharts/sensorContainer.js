import React, { useState } from "react";
import PropTypes from "prop-types";
import { Accordion, Icon, Message } from "semantic-ui-react";
import { genericApi } from "../../../api/genericApi";
import { API } from "../../../constants/appConstants";
import Chart from "./chart";

const SensorContainer = ({ sensor, onEditClicked }) => {
  const [visible, setVisible] = useState(false);
  const [data, setData] = useState([]);

  const handleExpand = async () => {
    setVisible(!visible);

    // we just expanded accordion
    if (visible === false) {
      const endpoint = API.SENSOR_DATA.GET_PAGED_FILTERED.format(sensor.sensorKey, 1, 2000);
      const result = await genericApi(endpoint, "GET");
      if (result.response && result.response.ok) {
        setData(result.body.values);
      }
    }
  };

  return (
    <div style={{ margin: "0 5px 10px 5px" }}>
      <Accordion fluid styled>
        <Accordion.Title active={visible} onClick={handleExpand}>
          <div className="inline">
            <Icon name="dropdown" />
            {sensor.name} {sensor.unit}
          </div>
          <div style={{ right: "2rem" }} className="inline absolute" onClick={e => onEditClicked(e, sensor.sensorKey)}>
            <Icon name="pencil" />
            Edit data
          </div>
        </Accordion.Title>
        <Accordion.Content active={visible} style={{ height: data.length === 0 ? "inherit" : "75vh" }}>
          {data.length === 0 ? (
            <Message color="orange">No data associated with sensor {sensor.name}</Message>
          ) : (
            <Chart data={data} sensor={sensor} />
          )}
        </Accordion.Content>
      </Accordion>
    </div>
  );
};

SensorContainer.propTypes = {
  sensor: PropTypes.shape({
    sensorKey: PropTypes.string.isRequired,
    name: PropTypes.string.isRequired,
    unit: PropTypes.string.isRequired
  }),
  onEditClicked: PropTypes.func.isRequired
};

export default SensorContainer;
