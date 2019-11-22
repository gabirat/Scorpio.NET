import React from "react";
import { bindActionCreators } from "redux";
import { connect } from "react-redux";
import { withRouter } from "react-router-dom";
import * as actions from "../../../actions";
import { Form as SemanticForm } from "semantic-ui-react";
import { Field } from "react-final-form";
import GenericWizard from "../../common/genericWizard";
import Validators from "../../../utils/formValidators";
import { mapDictionaryToDropdownOptions } from "../../../utils/mappings";

class ScheduleWizard extends React.Component {
  constructor(props) {
    super(props);
    this.state = { form: null };
  }

  onFormChanged(data) {
    this.setState({ form: data });
  }

  onSubmit(data) {
    this.props.onSubmit(data);
  }

  render() {
    const { onClose, initialValues, state, actions } = this.props;
    const { form } = this.state;

    const values = initialValues ? initialValues.props : null;

    return (
      <GenericWizard
        title={values ? "Edit schedule" : "Add schedule"}
        onClose={onClose}
        initialValues={values}
        onSubmit={this.onSubmit.bind(this)}
        onFormChanged={this.onFormChanged.bind(this)}
      >
        <GenericWizard.Page title="Name & Type">
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
          <Field name="jobTypeId" validate={Validators.required}>
            {({ input, meta }) => (
              <SemanticForm.Select
                {...input}
                label="Select job type"
                error={meta.invalid && meta.touched && meta.error}
                placeholder="Job type..."
                required
                onChange={(ev, data) => input.onChange(data.value)}
                options={mapDictionaryToDropdownOptions(state.dictionaries, "JobType")}
              />
            )}
          </Field>
        </GenericWizard.Page>

        <GenericWizard.Page title="Schedule">
          <Field name="cronExpression" validate={Validators.required}>
            {({ input, meta }) => (
              <SemanticForm.Input
                {...input}
                label="Cron expression i.e. 0/20 * * * * ?"
                error={meta.invalid && meta.touched && meta.error}
                placeholder="Enter cron expression..."
                required
                onChange={(ev, data) => input.onChange(data.value)}
              />
            )}
          </Field>
        </GenericWizard.Page>

        <GenericWizard.Page title="Parameters">
          <Field name="nodeId" validate={Validators.required}>
            {({ input, meta }) => (
              <SemanticForm.Select
                {...input}
                label="Select device"
                error={meta.invalid && meta.touched && meta.error}
                placeholder="Device..."
                required
                onChange={(ev, data) => input.onChange(data.value)}
                options={state.nodes.map(x => {
                  return {
                    key: x.id,
                    text: x.name,
                    value: x.id,
                    description: x.description
                  };
                })}
              />
            )}
          </Field>
          <Field name="command" validate={Validators.required}>
            {({ input, meta }) => {
              let commandDict = "";
              const node = state.nodes.find(x => x.id === form.values.nodeId);
              if (node) {
                const controlStrategyId = node.controlStrategyId;
                if (controlStrategyId) {
                  const strategy = state.controlStrategies.find(x => x.id === controlStrategyId);
                  if (strategy && strategy.connector) {
                    commandDict = `${strategy.connector}-commands`;
                    if (!state.dictionaries.find(x => x.name === commandDict)) actions.fetchDictionary(commandDict);
                  }
                }
              }

              const disabled = !node;

              return disabled ? null : (
                <SemanticForm.Select
                  {...input}
                  label="Select job type"
                  error={meta.invalid && meta.touched && meta.error}
                  placeholder="Job type..."
                  required
                  onChange={(ev, data) => input.onChange(data.value)}
                  options={mapDictionaryToDropdownOptions(state.dictionaries, commandDict)}
                />
              );
            }}
          </Field>
          <Field name="commandParams">
            {({ input, meta }) => {
              const node = state.nodes.find(x => x.id === form.values.nodeId);
              const disabled = !node;

              return disabled ? null : (
                <SemanticForm.Input
                  {...input}
                  label="Command parameters"
                  error={meta.invalid && meta.touched && meta.error}
                  placeholder="Enter additional commmand parameters..."
                  onChange={(ev, data) => input.onChange(data.value)}
                />
              );
            }}
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

export default withRouter(
  connect(
    mapStateToProps,
    mapDispatchToProps
  )(ScheduleWizard)
);
