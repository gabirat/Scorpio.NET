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
import StreamEditorWizard from "./streamEditorWizard";

const entityName = "stream";

class SensorEditorScreen extends Component {
  constructor(props) {
    super(props);
    this.state = { entities: [], isFetched: false, currentPage: 1, itemsPerPage: 25, editingEntity: null };
  }

  async componentDidMount() {
    const { currentPage, itemsPerPage } = this.state;
    await this.fetchItems(currentPage, itemsPerPage);
  }

  fetchItems = async (currentPage, itemsPerPage) => {
    this.setState({ isFetched: false });
    const result = await genericApi(API.STREAMS.GET_PAGED.format(currentPage, itemsPerPage), "GET");
    if (result.response.ok) {
      this.setState({ entities: result.body.values, isFetched: true, runWizard: false });
    }
    this.props.actions.setStreams(result.body.values); // this will be limited by paging - its ok for now
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
    if (window.confirm(`Are you sure you want to remove ${entityName} ${entity.name}?`)) {
      const { currentPage, itemsPerPage } = this.state;
      await genericApi(API.STREAMS.DELETE.format(entity.id), "DELETE");
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
    const url = isUpdate ? API.STREAMS.UPDATE.format(data.id) : API.STREAMS.ADD;
    await genericApi(url, isUpdate ? "PUT" : "POST", data);
    await this.fetchItems(currentPage, itemsPerPage);
    this.setState({ editingEntity: null });
  };

  render() {
    const { isFetched, entities, runWizard, itemsPerPage, currentPage, editingEntity } = this.state;
    const hasData = Array.isArray(entities) && entities.length > 0;

    return (
      <>
        {runWizard && <StreamEditorWizard initialValues={editingEntity} onClose={this.onCloseWizard} onSubmit={this.onWizardFinished} />}
        <Segment attached="bottom" style={{ padding: "1em" }}>
          <AddButtonMenuContainer addText={`Add new ${entityName}`} onAddClick={() => this.handleAddClick()}>
            {isFetched ? (
              <>
                {hasData ? (
                  <Table selectable celled color="orange">
                    <Table.Header>
                      <Table.Row>
                        <Table.HeaderCell width="2">Id</Table.HeaderCell>
                        <Table.HeaderCell>Name</Table.HeaderCell>
                        <Table.HeaderCell>Uri</Table.HeaderCell>
                        <Table.HeaderCell width="2">Actions</Table.HeaderCell>
                      </Table.Row>
                    </Table.Header>
                    <Table.Body>
                      {entities.map(x => {
                        return (
                          <Table.Row key={x.id}>
                            <TableCell>{x.id}</TableCell>
                            <TableCell>{x.name}</TableCell>
                            <TableCell>{x.uri}</TableCell>
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
                  <Message
                    color="yellow"
                    header={`There is no ${entityName} yet`}
                    list={[`You must add new ${entityName} to see the list here.`]}
                  />
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
