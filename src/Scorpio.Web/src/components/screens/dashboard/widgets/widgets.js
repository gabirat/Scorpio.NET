import SingleChart from "./singleChart";
import NumericStatistic from "./numericStatistic";

export const widgets = [
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
