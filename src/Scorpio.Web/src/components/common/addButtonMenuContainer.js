import React, { Component } from "react";
import PropTypes from "prop-types";
import { Icon, Menu, Segment } from "semantic-ui-react";

class AddButtonMenuContainer extends Component {
  render() {
    const { onAddClick, children, readOnly, addText, customLeftItem } = this.props;

    return (
      <>
        {!readOnly && (
          <Menu attached="top">
            <Menu.Item name="addNew" onClick={onAddClick}>
              <Icon size="large" name="add" />
              {addText ? addText : "Add new entity"}
            </Menu.Item>
            {customLeftItem}
          </Menu>
        )}
        <Segment attached="bottom">{children}</Segment>
      </>
    );
  }
}

export default AddButtonMenuContainer;

AddButtonMenuContainer.propTypes = {
  readOnly: PropTypes.bool,
  onAddClick: PropTypes.func.isRequired,
  customLeftItem: PropTypes.object
};
