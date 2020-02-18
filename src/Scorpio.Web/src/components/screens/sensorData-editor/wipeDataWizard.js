import React from "react";
import { withRouter } from "react-router-dom";
import { connect } from "react-redux";
import { bindActionCreators } from "redux";
import * as actions from "../../../actions";
import { Form as SemanticForm, FormField, Message } from "semantic-ui-react";
import { Field } from "react-final-form";
import GenericWizard from "../../common/genericWizard";
import Validators from "../../../utils/formValidators";
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";

class WipeDataWizard extends React.Component {
  onSubmit = data => {
    const { onSubmit } = this.props;

    if (!data.from) data.from = null;
    else if (typeof data.from.toISOString === "function") data.from = data.from.toISOString();

    if (!data.to) data.to = null;
    else if (typeof data.to.toISOString === "function") data.to = data.to.toISOString();

    if (typeof onSubmit === "function") onSubmit(data);
  };

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
    const { onClose } = this.props;

    return (
      <GenericWizard title={"Wipe sensor data"} onClose={onClose} initialValues={null} onSubmit={this.onSubmit} showSteps={false}>
        <GenericWizard.Page title="Data">
          <Message>This operation is irreversible. If no date is specified, all the values will be purged.</Message>

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

          <Field name="from">
            {({ input, meta }) => (
              <FormField
                label="Date from"
                error={meta.invalid && meta.touched && meta.error}
                control={() => (
                  <DatePicker
                    className="cursor-text"
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
          <Field name="to">
            {({ input, meta }) => (
              <FormField
                label="Date to"
                error={meta.invalid && meta.touched && meta.error}
                control={() => (
                  <DatePicker
                    className="cursor-text"
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

export default withRouter(connect(mapStateToProps, mapDispatchToProps)(WipeDataWizard));
