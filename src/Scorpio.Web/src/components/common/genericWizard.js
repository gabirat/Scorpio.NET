import React, { Component } from "react";
import PropTypes from "prop-types";
import { Form, FormSpy } from "react-final-form";
import { Form as SemanticForm, Modal, Header, Divider, Button, Segment } from "semantic-ui-react";
import Stepper from "./stepper/stepper";

export default class GenericWizard extends Component {
  static propTypes = {
    onSubmit: PropTypes.func.isRequired
  };

  static Page = ({ children }) => children;

  constructor(props) {
    super(props);

    this.state = {
      page: 0,
      values: props.initialValues || {}
    };
  }

  next = values => {
    this.setState(state => ({
      page: Math.min(state.page + 1, this.props.children.length - 1),
      values
    }));
  };

  previous = ev => {
    if (ev) ev.preventDefault(); // disable submit
    this.setState({ page: Math.max(this.state.page - 1, 0) });
  };

  /**
   * NOTE: Both validate and handleSubmit switching are implemented
   * here because 🏁 Redux Final Form does not accept changes to those
   * functions once the form has been defined.
   */
  validate = values => {
    const activePage = React.Children.toArray(this.props.children)[this.state.page];
    return activePage.props.validate ? activePage.props.validate(values) : {};
  };

  handleFinalFormSubmit = values => {
    const { children, onSubmit } = this.props;
    const isLastPage = this.state.page === React.Children.count(children) - 1;
    if (isLastPage) {
      return onSubmit(values);
    } else {
      this.next(values);
    }
  };

  onFormChanged = formData => {
    this.formData = formData;
    if (this.props.onFormChanged) this.props.onFormChanged(formData);
  };

  onStepperClicked(indx) {
    const { page } = this.state;
    const deltaPages = indx - page;

    // deltaPages<0 => moving backward
    if (deltaPages < 0) {
      this.setState({ page: Math.min(0, -deltaPages) });
    }
  }

  render() {
    let { children, onClose, title, size, showSteps } = this.props;
    const { page, values } = this.state;
    const currentPage = React.Children.toArray(children)[page];
    const isLastPage = page === React.Children.count(children) - 1;
    const steps = React.Children.toArray(children)
      .map(x => x.props)
      .map((x, indx) =>
        x ? { title: x.title, onClick: _ => this.onStepperClicked(indx) } : { title: "", onClick: _ => this.onStepperClicked(indx) }
      );

    if (showSteps === undefined) showSteps = true; //defaults to true

    return (
      <Modal
        mountNode={document.getElementById("main-content")}
        size={size ? size : "tiny"}
        dimmer={"inverted"}
        open={true}
        onClose={onClose}
      >
        <Header
          content={
            <>
              <Header as="h3" icon="add" content={title} />
              {showSteps && <Stepper steps={steps} activeStep={page} />}
            </>
          }
        />
        <Modal.Content>
          <Modal.Description>
            <Form initialValues={values} validate={this.validate} onSubmit={this.handleFinalFormSubmit}>
              {({ handleSubmit, submitting, invalid }) => (
                <>
                  <FormSpy subscription={{ values: true, valid: true, errors: true, dirtyFields: true }} onChange={this.onFormChanged} />
                  <SemanticForm onSubmit={handleSubmit} autoComplete="true" noValidate>
                    {currentPage}
                    <Divider />
                    <Segment basic style={{ marginBottom: "15px" }}>
                      {page > 0 && (
                        <Button floated="left" onClick={this.previous}>
                          Back
                        </Button>
                      )}
                      <Button floated="right" primary disabled={submitting || invalid}>
                        {isLastPage ? "Submit" : "Continue"}
                      </Button>
                    </Segment>
                  </SemanticForm>
                </>
              )}
            </Form>
          </Modal.Description>
        </Modal.Content>
      </Modal>
    );
  }
}
