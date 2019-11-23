import { store } from "../App";
import AlertDispatcher from "../services/AlertDispatcher";
import LogService from "../services/LogService";

export async function genericApi(endpoint, verb, data, actionToDispatchOnSuccess, actionToDispatchOnError) {
  switch (verb) {
    case "GET":
    case "PUT":
    case "POST":
    case "DELETE":
      return await fetch(endpoint, getFetchParam(verb, data))
        .then(response => handleResponse(response, actionToDispatchOnSuccess))
        .catch(err => handleError(err, actionToDispatchOnError));

    default:
      throw AlertDispatcher.dispatchError("genericApi: Unkown verb", verb);
  }
}

function getFetchParam(verb, body) {
  return {
    method: verb,
    headers: {
      "Content-type": "application/json; charset=UTF-8",
      Accept: "application/json"
    },
    body: (verb === "POST" || verb === "PUT") && body ? JSON.stringify(body) : undefined
  };
}

function handleResponse(response, actionToDispatchOnSuccess) {
  if (response) {
    return response.json().then(data => {
      dispatchApiAlertsIfExists(data);
      tryDispatchAction(store, actionToDispatchOnSuccess, data.data);
      return {
        response,
        body: data
      };
    });
  } else {
    throw new Error("Response is invalid");
  }
}

function dispatchApiAlertsIfExists(responseBody) {
  if (responseBody && Array.isArray(responseBody.alerts) && responseBody.alerts.length > 0) {
    const mappedAlerts = responseBody.alerts.map(alert => {
      return {
        type: alert.type,
        text: alert.message
      };
    });
    AlertDispatcher.dispatch(mappedAlerts);
  }
}

function handleError(err, actionToDispatchOnError) {
  AlertDispatcher.dispatchError("CRITICAL: Ooops, could not execute AJAX request to API. Check if API is running.");
  LogService.error("genericApi", err);
  tryDispatchAction(store, actionToDispatchOnError, err);
  throw err; // bubble error
}

function tryDispatchAction(store, action, data) {
  if (action && typeof action === "function") store.dispatch(action(data));
}
