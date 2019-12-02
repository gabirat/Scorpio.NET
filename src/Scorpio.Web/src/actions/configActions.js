import * as types from "../actions/actionTypes";
import { guid } from "../utils/utils";

// initially set all configs from user data
export function setConfigs(configs) {
  return {
    type: types.SET_CONFIGS,
    payload: configs
  };
}

// it triggers saga to POST it to backend
export function setConfig(config) {
  if (config.id === 0) config.id = guid(); // temporary - because we dont have ID assigned by backend yet
  return {
    type: types.SET_CONFIG,
    payload: {
      config
    }
  };
}

// consumed by saga, to replace temprary set guid to databse id after API call
export function replaceGuidByDbId(configType, oldGuid, newId) {
  return {
    type: types.REPLACE_CONFIG_ID,
    payload: {
      configType,
      oldGuid,
      newId
    }
  };
}

// triggers saga to DELETE it at backend side
export function deleteConfig(id) {
  return {
    type: types.DELETE_CONFIG,
    payload: id
  };
}
