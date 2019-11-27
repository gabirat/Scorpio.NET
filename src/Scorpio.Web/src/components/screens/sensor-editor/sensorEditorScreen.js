import React, { Component } from "react";
import { connect } from "react-redux";
import { bindActionCreators } from "redux";
import { withRouter } from "react-router-dom";
import { Segment, Message, Table, TableCell, Button } from "semantic-ui-react";
import * as actions from "../../../actions";
import { genericApi } from "../../../api/genericApi";
import { API } from "../../../constants/appConstants";
import AddButtonMenuContainer from "../../common/addButtonMenuContainer";
import Spinner from "../../common/spinner";
import Pager from "../../common/pager";
import SensorWizard from "./sensorEditorWizard";

class SensorEditorScreen extends Component {
  constructor(props) {
    super(props);
    this.state = { sensors: [], isFetched: false, currentPage: 1, itemsPerPage: 25, editingEntity: null };
  }

  async componentDidMount() {
    const { currentPage, itemsPerPage } = this.state;
    await this.fetchItems(currentPage, itemsPerPage);
  }

  fetchItems = async (currentPage, itemsPerPage) => {
    this.setState({ isFetched: false });
    const result = await genericApi(API.SENSORS.GET_PAGED.format(currentPage, itemsPerPage), "GET");
    if (result.response.ok) {
      this.setState({ sensors: result.body.values, isFetched: true, runWizard: false });
    }
  };

  onItemsPerPageChanged = async itemsPerPage => {
    const { currentPage } = this.state;
    this.setState({ isFetched: false, itemsPerPage: itemsPerPage });
    await this.fetchItems(currentPage, itemsPerPage);
  };

  onPageChange = async pageNum => {
    const { itemsPerPage } = this.state;
    this.setState({ isFetched: false, currentPage: pageNum });
    await this.fetchItems(pageNum, itemsPerPage);
  };

  handleRemoveClick = async entity => {
    if (window.confirm(`Are you sure you want to remove sensor ${entity.name}?`)) {
      const { currentPage, itemsPerPage } = this.state;
      await genericApi(API.SENSORS.DELETE.format(entity.id), "DELETE");
      await this.fetchItems(currentPage, itemsPerPage);
    }
  };

  handleEditClick = entity => {
    this.setState({ runWizard: true, editingEntity: entity });
  };

  handleAddClick = () => {
    this.setState({ runWizard: true });
  };

  onWizardFinished = async data => {
    const { editingEntity, currentPage, itemsPerPage } = this.state;
    const isUpdate = editingEntity !== null;
    const url = isUpdate ? API.SENSORS.UPDATE.format(data.id) : API.SENSORS.ADD;
    await genericApi(url, isUpdate ? "PUT" : "POST", data);
    await this.fetchItems(currentPage, itemsPerPage);
    this.setState({ editingEntity: null });
  };

  render() {
    const { isFetched, sensors, runWizard, itemsPerPage, currentPage, editingEntity } = this.state;
    const hasData = Array.isArray(sensors) && sensors.length > 0;

    return (
      <>
        {runWizard && <SensorWizard initialValues={editingEntity} onClose={this.onCloseWizard} onSubmit={this.onWizardFinished} />}
        <Segment attached="bottom" style={{ padding: "1em" }}>
          <AddButtonMenuContainer addText={"Add new sensor"} onAddClick={() => this.handleAddClick()}>
            {isFetched ? (
              <>
                {hasData ? (
                  <Table selectable celled color="orange">
                    <Table.Header>
                      <Table.Row>
                        <Table.HeaderCell width="2">Id</Table.HeaderCell>
                        <Table.HeaderCell>Name</Table.HeaderCell>
                        <Table.HeaderCell>Key</Table.HeaderCell>
                        <Table.HeaderCell>Unit</Table.HeaderCell>
                        <Table.HeaderCell width="2">Actions</Table.HeaderCell>
                      </Table.Row>
                    </Table.Header>
                    <Table.Body>
                      {sensors.map(x => {
                        return (
                          <Table.Row key={x.id}>
                            <TableCell>{x.id}</TableCell>
                            <TableCell>{x.name}</TableCell>
                            <TableCell>{x.sensorKey}</TableCell>
                            <TableCell>{x.unit}</TableCell>
                            <TableCell>
                              <Button icon="edit" color="grey" onClick={() => this.handleEditClick(x)} />
                              <Button icon="remove" color="red" onClick={() => this.handleRemoveClick(x)} />
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
            <Pager
              currentPage={currentPage}
              itemsPerPage={itemsPerPage}
              onItemsPerPageChanged={this.onItemsPerPageChanged}
              onPageChange={this.onPageChange}
            />
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

export default withRouter(connect(mapStateToProps, mapDispatchToProps)(SensorEditorScreen));
