import { ConfigActionTypes } from "../actions/configActions";
import { IConfigModel } from "../../models/configModel";

export type ConfigState = Readonly<{
  configs: IConfigModel[];
}>;

const initialState: ConfigState = {
  configs: []
};

export function configReducer(state = initialState, action: ConfigActionTypes): ConfigState {
  switch (action.type) {
    default:
      return state;
  }
}
