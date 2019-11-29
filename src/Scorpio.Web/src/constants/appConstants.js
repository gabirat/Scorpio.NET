// API routes
const baseUrl = process.env.REACT_APP_BACKEND_URL;

export const API = {
  ROOT: baseUrl + "/",
  SIGNALR: baseUrl + "/hub",
  CONFIG: {
    GET_ALL: baseUrl + "/api/configuration",
    POST_NEW: baseUrl + "/api/configuration",
    UPDATE_BY_ID: baseUrl + "/api/configuration/{0}",
    DELETE_BY_ID: baseUrl + "/api/configuration/{0}"
  },
  SENSORS: {
    GET_ALL: baseUrl + "/api/sensor",
    GET_PAGED: baseUrl + "/api/sensor/paged?pageNumber={0}&itemsPerPage={1}",
    UPDATE: baseUrl + "/api/sensor/{0}",
    ADD: baseUrl + "/api/sensor",
    DELETE: baseUrl + "/api/sensor/{0}"
  },
  SENSOR_DATA: {
    GET_ALL: baseUrl + "/api/sensorData",
    GET_ALL_BY_KEY: baseUrl + "/api/sensorData/sensorKey/{0}",
    GET_PAGED: baseUrl + "/api/sensorData/paged?pageNumber={0}&itemsPerPage={1}",
    UPDATE: baseUrl + "/api/sensorData/{0}",
    ADD: baseUrl + "/api/sensorData",
    DELETE: baseUrl + "/api/sensorData/{0}"
  },
  STREAMS: {
    GET_ALL: baseUrl + "/api/stream",
    GET_PAGED: baseUrl + "/api/stream/paged?pageNumber={0}&itemsPerPage={1}",
    UPDATE: baseUrl + "/api/stream/{0}",
    ADD: baseUrl + "/api/stream",
    DELETE: baseUrl + "/api/stream/{0}"
  }
};
