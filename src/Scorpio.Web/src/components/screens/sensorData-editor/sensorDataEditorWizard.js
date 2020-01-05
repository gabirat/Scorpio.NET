import React from "react";
import { withRouter } from "react-router-dom";
import { connect } from "react-redux";
import { bindActionCreators } from "redux";
import * as actions from "../../../actions";
import { Form as SemanticForm, FormField } from "semantic-ui-react";
import { Field } from "react-final-form";
import GenericWizard from "../../common/genericWizard";
import Validators from "../../../utils/formValidators";
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";

class SensorDataEditorWizard extends React.Component {
  onSubmit(data) {
    this.props.onSubmit(data);
  }

  getSensorDropdownOptions = () => {
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

  render() {
    const { onClose, initialValues } = this.props;

    return (
      <GenericWizard
        title={initialValues ? "Edit data point" : "Add data point"}
        onClose={onClose}
        initialValues={initialValues}
        onSubmit={this.onSubmit.bind(this)}
        showSteps={false}
      >
        <GenericWizard.Page title="Data">
          <Field name="sensorKey" validate={Validators.required}>
            {({ input, meta }) => (
              <SemanticForm.Select
                {...input}
                label="Sensor Key"
                error={meta.invalid && meta.touched && meta.error}
                placeholder="Sensor Key..."
                search
                options={this.getSensorDropdownOptions()}
                required
                onChange={(ev, data) => input.onChange(data.value)}
              />
            )}
          </Field>
          <Field name="timeStamp" validate={Validators.required}>
            {({ input, meta }) => (
              <FormField
                label="Time"
                required
                error={meta.invalid && meta.touched && meta.error}
                control={() => (
                  <DatePicker
                    {...input}
                    selected={input.value ? new Date(input.value) : null}
                    onChange={date => input.onChange(date)}
                    showTimeSelect
                    isClearable
                    allowSameDay
                    placeholderText="Select date"
                    timeFormat="HH:mm"
                    timeIntervals={1}
                    timeCaption="Time"
                    dateFormat="MMMM d, yyyy h:mm aa"
                  />
                )}
              />
            )}
          </Field>
          <Field name="value" validate={Validators.required}>
            {({ input, meta }) => (
              <SemanticForm.Input
                {...input}
                label="Value"
                error={meta.invalid && meta.touched && meta.error}
                placeholder="Value..."
                required
                onChange={(ev, data) => input.onChange(data.value)}
              />
            )}
          </Field>
          <Field name="comment">
            {({ input, meta }) => (
              <SemanticForm.Input
                {...input}
                label="Comment"
                error={meta.invalid && meta.touched && meta.error}
                placeholder="Comment..."
                onChange={(ev, data) => input.onChange(data.value)}
              />
            )}
          </Field>
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

export default withRouter(connect(mapStateToProps, mapDispatchToProps)(SensorDataEditorWizard));
