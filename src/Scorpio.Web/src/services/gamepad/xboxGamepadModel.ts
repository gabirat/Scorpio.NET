export interface IXboxGamepadModel {
  index: number;
  name: string;
  leftStick: IStickModel;
  rightStick: IStickModel;
  leftTrigger: number;
  rightTrigger: number;
}

export interface IStickModel {
  xVal: number;
  yVal: number;
}
