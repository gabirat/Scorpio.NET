import { put, takeLatest } from "redux-saga/effects";
import { genericApi } from "../api/genericApi";
import { API } from "../constants/appConstants";
import { replaceGuidByDbId } from "../actions";
import { SET_CONFIG } from "../actions/actionTypes";
import AlertDispatcher from "../services/AlertDispatcher";
import LogService from "../services/LogService";

export function* addUpdateConfigSaga() {
  yield takeLatest(SET_CONFIG, notifyApi);
}

function* notifyApi(action) {
  try {
    const body = getRequestBody(action.payload);

    // If its add, then we have GUID, else we got databse ID (string without dash)
    const configId = action.payload.config.id;
    const isUpdate = configId && typeof configId === "string" && configId.indexOf("-") === -1;

    if (isUpdate) {
      yield genericApi(API.CONFIG.UPDATE_BY_ID.format(configId), "PUT", body);
    } else {
      const tempGuid = action.payload.config.id;
      delete body.id;
      const result = yield genericApi(API.CONFIG.POST_NEW, "POST", body);
      yield put(replaceGuidByDbId("member", tempGuid, result.body.data.id));
    }
  } catch (err) {
    LogService.error("addUpdateWidgetConfigSaga", err);
    AlertDispatcher.dispatchError("Could not save configuration");
  }
}

function getRequestBody(payload) {
  const configCopy = Object.assign({}, payload.config);

  return {
    ...configCopy,
    data: JSON.stringify(configCopy.data)
  };
}
