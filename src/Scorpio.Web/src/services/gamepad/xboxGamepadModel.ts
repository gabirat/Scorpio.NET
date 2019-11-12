export interface IXboxGamepadModel {
  index: Number;
  name: string;
  leftStick: IStickModel;
  rightStick: IStickModel;
  leftTrigger: Number;
  rightTrigger: Number;
}

interface IStickModel {
  xVal: Number;
  yVal: Number;
}
