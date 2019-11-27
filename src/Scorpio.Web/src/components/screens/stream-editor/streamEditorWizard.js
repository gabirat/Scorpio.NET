import React from "react";
import { bindActionCreators } from "redux";
import { connect } from "react-redux";
import { withRouter } from "react-router-dom";
import * as actions from "../../../actions";
import { Form as SemanticForm } from "semantic-ui-react";
import { Field } from "react-final-form";
import GenericWizard from "../../common/genericWizard";
import Validators from "../../../utils/formValidators";

class StreamEditorWizard extends React.Component {
  onSubmit(data) {
    this.props.onSubmit(data);
  }

  render() {
    const { onClose, initialValues } = this.props;

    return (
      <GenericWizard
        title={initialValues ? "Edit stream" : "Add stream"}
        onClose={onClose}
        initialValues={initialValues}
        onSubmit={this.onSubmit.bind(this)}
        showSteps={false}
      >
        <GenericWizard.Page title="Data">
          <Field name="name" validate={Validators.required}>
            {({ input, meta }) => (
              <SemanticForm.Input
                {...input}
                label="Name"
                error={meta.invalid && meta.touched && meta.error}
                placeholder="Name..."
                required
                onChange={(ev, data) => input.onChange(data.value)}
              />
            )}
          </Field>
          <Field name="sensorKey" validate={Validators.required}>
            {({ input, meta }) => (
              <SemanticForm.Input
                {...input}
                label="Sensor Key"
                error={meta.invalid && meta.touched && meta.error}
                placeholder="Sensor Key..."
                required
                onChange={(ev, data) => input.onChange(data.value)}
              />
            )}
          </Field>
          <Field name="unit" validate={Validators.required}>
            {({ input, meta }) => (
              <SemanticForm.Input
                {...input}
                label="Unit"
                error={meta.invalid && meta.touched && meta.error}
                placeholder="Unit..."
                required
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

export default withRouter(connect(mapStateToProps, mapDispatchToProps)(StreamEditorWizard));
