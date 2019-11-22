import React, { Component } from "react";
import { Segment, Message, Table, TableCell, Button } from "semantic-ui-react";
import AddButtonMenuContainer from "../../common/addButtonMenuContainer";
import Spinner from "../../common/spinner";
import { genericApi } from "../../../api/genericApi";
import { API } from "../../../constants/appConstants";
import { connect } from "react-redux";
import { bindActionCreators } from "redux";
import { withRouter } from "react-router-dom";
import * as actions from "../../../actions";
import moment from "moment";
import { mapDictionaryToValue } from "../../../utils/mappings";
import ScheduleWizard from "./scheduleWizard";
import cronstrue from "cronstrue";

class ScheduleScreen extends Component {
  constructor(props) {
    super(props);
    this.state = { schedules: [], isFetched: false, runWizard: false };
  }

  componentDidMount() {
    this.fetchAll();
  }

  async fetchAll() {
    this.props.actions.fetchDictionary("JobType");
    this.props.actions.fetchDictionary("JobStatus");

    const result = await genericApi(API.SCHEDULE.GET_ALL, "GET");
    const { response, body } = result;
    if (response.ok && body && body.data) {
      this.setState({ schedules: body.data, isFetched: true });
    }
  }

  handleAddClick = () => {
    this.setState({ runWizard: true });
  };

  handleJobPauseResumeClick = async (jobId, currentStatus) => {
    let newStatus = currentStatus === 10 ? 1 : 10;
    await genericApi(API.SCHEDULE.UPDATE_STATUS_BY_ID.format(jobId, newStatus), "PUT");
    this.fetchAll();
  };

  handleRemoveJobClick = async jobId => {
    if (window.confirm("Are you sure?")) {
      await genericApi(API.SCHEDULE.DELETE_BY_ID.format(jobId), "DELETE");
      this.fetchAll();
    }
  };

  onWizardFinished = async data => {
    this.onCloseWizard();
    await genericApi(API.SCHEDULE.NEW_NODE_CMD, "POST", data);
    this.fetchAll();
  };

  onCloseWizard = () => {
    this.setState({ runWizard: false });
  };

  render() {
    const { dictionaries } = this.props.state;
    const { isFetched, schedules, runWizard } = this.state;
    const hasData = schedules && schedules.length > 0;

    return (
      <>
        {runWizard && <ScheduleWizard onClose={this.onCloseWizard} onSubmit={this.onWizardFinished} />}
        <Segment attached="bottom" style={{ padding: "1em" }}>
          <AddButtonMenuContainer addText={"Add new schedule"} onAddClick={() => this.handleAddClick()}>
            {isFetched ? (
              <>
                {hasData ? (
                  <Table selectable celled color="teal">
                    <Table.Header>
                      <Table.Row>
                        <Table.HeaderCell>Id</Table.HeaderCell>
                        <Table.HeaderCell>Name</Table.HeaderCell>
                        <Table.HeaderCell>Status</Table.HeaderCell>
                        <Table.HeaderCell>Type</Table.HeaderCell>
                        <Table.HeaderCell>Schedule</Table.HeaderCell>
                        <Table.HeaderCell>Created</Table.HeaderCell>
                        <Table.HeaderCell>Created by</Table.HeaderCell>
                        <Table.HeaderCell>Actions</Table.HeaderCell>
                      </Table.Row>
                    </Table.Header>
                    <Table.Body>
                      {schedules.map(x => {
                        return (
                          <Table.Row key={x.id}>
                            <TableCell>{x.id}</TableCell>
                            <TableCell>{x.name}</TableCell>
                            <TableCell>{mapDictionaryToValue(dictionaries, "JobStatus", x.jobStatusId)}</TableCell>
                            <TableCell>{mapDictionaryToValue(dictionaries, "JobType", x.jobTypeId)}</TableCell>
                            <TableCell>{cronstrue.toString(x.cronExpression ? x.cronExpression : "")}</TableCell>
                            <TableCell>{moment(x.created).format("MMMM Do YYYY, h:mm:ss a")}</TableCell>
                            <TableCell>{x.createdBy}</TableCell>
                            <TableCell>
                              <Button
                                color={x.jobStatusId === 1 ? "grey" : "green"}
                                icon={x.jobStatusId === 1 ? "pause" : "play"}
                                onClick={() => this.handleJobPauseResumeClick(x.id, x.jobStatusId)}
                              />
                              <Button icon="remove" color="red" onClick={() => this.handleRemoveJobClick(x.id)} />
                            </TableCell>
                          </Table.Row>
                        );
                      })}
                    </Table.Body>
                  </Table>
                ) : (
                  <Message color="yellow" header="There is no entities yet" list={["You must add new entities to see the list here."]} />
                )}
              </>
            ) : (
              <Spinner />
            )}
          </AddButtonMenuContainer>
        </Segment>
      </>
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

export default withRouter(connect(mapStateToProps, mapDispatchToProps)(ScheduleScreen));
