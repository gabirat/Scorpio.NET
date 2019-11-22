// API routes
const baseUrl = process.env.REACT_APP_BACKEND_URL;

export const API = {
  CONFIG: {
    GET_ALL: baseUrl + "/api/configuration",
    POST_NEW: baseUrl + "/api/configuration",
    UPDATE_BY_ID: baseUrl + "/api/configuration/{0}",
    DELETE_BY_ID: baseUrl + "/api/configuration/{0}"
  }
};
