export const FETCH_CONFIGS = "FETCH_CONFIGS";

export interface FetchConfigsAction {
  type: typeof FETCH_CONFIGS;
}

export const fetchConfigs = (): FetchConfigsAction => {
  return {
    type: "FETCH_CONFIGS"
  };
};

export type ConfigActionTypes = FetchConfigsAction;
