import React, { Component } from "react";
import { Message, Table, TableCell, Button } from "semantic-ui-react";

class GenericGrid extends Component {
  constructor(props) {
    super(props);
  }

  render() {
    const { values, config } = this.props;
    const hasData = Array.isArray(values) && values.length > 0;

    return (
      <>
        {hasData ? (
          <Table selectable celled color="teal">
            <Table.Header>
              <Table.Row>
                <Table.HeaderCell>Id</Table.HeaderCell>
                <Table.HeaderCell>Name</Table.HeaderCell>
                <Table.HeaderCell>Connector</Table.HeaderCell>
                <Table.HeaderCell>Description</Table.HeaderCell>
                <Table.HeaderCell>Properties</Table.HeaderCell>
                <Table.HeaderCell>Created</Table.HeaderCell>
                <Table.HeaderCell>Created by</Table.HeaderCell>
                <Table.HeaderCell>Actions</Table.HeaderCell>
              </Table.Row>
            </Table.Header>
            <Table.Body>
              {entities.map(x => {
                return (
                  <Table.Row key={x.id}>
                    <TableCell width="1" textAlign="center">
                      {x.id}
                    </TableCell>
                    <TableCell width="2">{x.name}</TableCell>
                    <TableCell width="2">{x.connector}</TableCell>
                    <TableCell>{x.description}</TableCell>
                    <TableCell>
                      <ListRenderer list={x.physicalProperties} idKey={"id"} textKey={"name"} />
                    </TableCell>
                    <TableCell width="2" textAlign="center">
                      {moment(x.created).format("MMMM Do YYYY, h:mm:ss a")}
                    </TableCell>
                    <TableCell width="2" textAlign="center">
                      {x.createdBy}
                    </TableCell>
                    <TableCell width="2" textAlign="center">
                      <Button icon="edit" color="grey" onClick={ev => this.handleEditClicked(ev, x)} />
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
    );
  }
}

export default GenericGrid;
