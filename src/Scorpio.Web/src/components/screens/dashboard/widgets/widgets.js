import StatisticWidget from "./statisticWidget";
import GamepadAnalogs from "./gamepadAnalogs";
import ChartWidget from "./chartWidget";

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
    component: ChartWidget,
    type: "Chart",
    dropdown: {
      key: "Chart",
      value: "Chart",
      text: "Line chart",
      description: "Plot a chart"
    }
  },
  {
    component: StatisticWidget,
    type: "StatisticWidget",
    dropdown: {
      key: "StatisticWidget",
      value: "StatisticWidget",
      text: "Single value",
      description: "Display last seen value"
    }
  }
];
