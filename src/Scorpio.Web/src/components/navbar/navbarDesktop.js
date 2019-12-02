import React from "react";
import { withRouter } from "react-router-dom";
import { Image, Menu, Icon } from "semantic-ui-react";
import MenuItems from "./menuItems";

const NavBarDesktop = ({ history }) => {
  const handleClick = (_, data) => {
    history.push(data.name);
  };

  return (
    <Menu fixed="top">
      <Menu.Item>
        <Image size="tiny" src={process.env.PUBLIC_URL + "/logo.png"} />
      </Menu.Item>
      <MenuItems onClick={handleClick} />
      <Menu.Item name="/about" as="a" position="right" onClick={handleClick}>
        <Icon name="info circle" />
        About
      </Menu.Item>
    </Menu>
  );
};

export default withRouter(NavBarDesktop);
