import SingleChart from "./singleChart";
import NumericStatistic from "./numericStatistic";
import GamepadAnalogs from "./gamepadAnalogs";

export const widgets = [
  {
    component: GamepadAnalogs,
    type: "GamepadAnalogs",
    dropdown: {
      key: "GamepadAnalogs",
      value: "GamepadAnalogs",
      text: "Gamepad analog previewer",
      description: "View pad state"
    }
  },
  {
    component: SingleChart,
    type: "SingleChart",
    dropdown: {
      key: "SingleChart",
      value: "SingleChart",
      text: "Line chart",
      description: "Plot a chart"
    }
  },
  {
    component: NumericStatistic,
    type: "NumericStatistic",
    dropdown: {
      key: "NumericStatistic",
      value: "NumericStatistic",
      text: "Single value panel",
      description: "Display last seen value"
    }
  }
];
