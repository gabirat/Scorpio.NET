import React from "react";
import { bindActionCreators } from "redux";
import { connect } from "react-redux";
import { withRouter } from "react-router-dom";
import * as actions from "../../../actions";
import { Form as SemanticForm } from "semantic-ui-react";
import { Field } from "react-final-form";
import GenericWizard from "../../common/genericWizard";
import Validators from "../../../utils/formValidators";
import { widgets } from "./widgets/widgets";

class DashboardWidgetWizard extends React.Component {
  constructor(props) {
    super(props);
    this.state = { form: null };
  }

  getWidgetsOptions = () => widgets.map(x => x.dropdown) || [];
  getSensorOptions = () => {
    const { sensors } = this.props.state;
    return Array.isArray(sensors)
      ? sensors.map(sensor => {
          return {
            key: sensor.sensorKey,
            value: sensor.sensorKey,
            text: sensor.name
          };
        })
      : [];
  };

  onFormChanged(data) {
    this.setState({ form: data });
  }

  onSubmit(data) {
    const config = this.mapFormToWidgetConfig(data);
    this.props.onSubmit(config);
  }

  mapFormToWidgetConfig = data => {
    return {
      type: data.widgetTtype,
      props: {
        ...data
      }
    };
  };

  render() {
    const { onClose, initialValues } = this.props;
    const { form } = this.state;
    const isAnalogGamepadWidget = form && form.values && form.values.widgetTtype === "GamepadAnalogs";
    const isChartWidget = form && form.values && form.values.widgetTtype === "Chart";
    const isStatisticWidget = form && form.values && form.values.widgetTtype === "StatisticWidget";

    const values = initialValues ? initialValues.props : null;

    return (
      <GenericWizard
        title={values ? "Edit widget" : "Add widget"}
        onClose={onClose}
        initialValues={values}
        onSubmit={this.onSubmit.bind(this)}
        onFormChanged={this.onFormChanged.bind(this)}
      >
        <GenericWizard.Page title="Widget type">
          <Field name="widgetTtype" validate={Validators.required}>
            {({ input, meta }) => (
              <SemanticForm.Select
                {...input}
                label="Select widget type"
                error={meta.invalid && meta.touched && meta.error}
                placeholder="Select widget type"
                required
                search
                onChange={(ev, data) => input.onChange(data.value)}
                options={this.getWidgetsOptions()}
              />
            )}
          </Field>

          <Field name="widgetTitle" validate={Validators.required}>
            {({ input, meta }) => (
              <SemanticForm.Input
                {...input}
                label="Widget title"
                placeholder="Input widget title"
                required
                error={meta.invalid && meta.touched && meta.error}
              />
            )}
          </Field>
        </GenericWizard.Page>

        {/* SECOND PAGE */}
        <GenericWizard.Page title="Widget properties">
          {/* GAMEPAD WIDGET SPECIFIC FIELDS */}
          {isAnalogGamepadWidget && (
            <Field name="gamepadIndex" validate={Validators.required}>
              {({ input, meta }) => (
                <SemanticForm.Input
                  {...input}
                  label="Gamepad index"
                  placeholder="Input gamepad index"
                  required
                  error={meta.invalid && meta.touched && meta.error}
                />
              )}
            </Field>
          )}

          {/* CHART WIDGET SPECIFIC FIELDS */}
          {(isChartWidget || isStatisticWidget) && (
            <Field name="sensorKey" validate={Validators.required}>
              {({ input, meta }) => (
                <SemanticForm.Select
                  {...input}
                  label="Select sensor"
                  error={meta.invalid && meta.touched && meta.error}
                  placeholder="Select sensor..."
                  required
                  search
                  onChange={(ev, data) => input.onChange(data.value)}
                  options={this.getSensorOptions()}
                />
              )}
            </Field>
          )}
        </GenericWizard.Page>
      </GenericWizard>
    );
  }
}

function mapStateToProps(state) {
  return { state };
}

function mapDispatchToProps(dispatch) {
  return {
    actions: bindActionCreators(actions, dispatch)
  };
}

export default withRouter(connect(mapStateToProps, mapDispatchToProps)(DashboardWidgetWizard));
