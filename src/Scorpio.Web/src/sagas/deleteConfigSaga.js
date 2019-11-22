import { takeLatest } from "redux-saga/effects";
import { genericApi } from "../api/genericApi";
import { API } from "../constants/appConstants";
import { DELETE_CONFIG } from "../actions/actionTypes";
import AlertDispatcher from "../services/AlertDispatcher";
import LogService from "../services/LogService";

export function* deleteConfigSaga() {
  yield takeLatest(DELETE_CONFIG, notifyApi);
}

function* notifyApi(action) {
  try {
    const url = API.CONFIG.UPDATE_BY_ID.format(action.payload);
    yield genericApi(url, "DELETE");
  } catch (err) {
    LogService.error("addUpdateWidgetConfigSaga", err);
    AlertDispatcher.dispatchError("Could not delete configuration");
  }
}
