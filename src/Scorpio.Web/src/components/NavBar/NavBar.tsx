import React, { Component } from "react";
import { Icon, Image, Menu, Sidebar, Responsive } from "semantic-ui-react";
import { withRouter, RouteComponentProps } from "react-router-dom";

interface NavBarItem {
  as: string;
  content: string;
  key: string;
  url: string;
}

interface NavBarProps {
  leftItems: NavBarItem[];
  rightItems: NavBarItem[];
  children?: any;
  height?: string | undefined;
}

interface RoutedNavBarProps extends RouteComponentProps<any>, NavBarProps {}

interface NavBarDesktopProps extends NavBarProps {
  changeNavBarCard: (url: string) => void;
}

interface NavBarMobileProps extends NavBarProps {
  onPusherClick: () => void;
  onToggle: () => void;
  visible: boolean;
  changeNavBarCard: (url: string) => void;
}

const defaultHeight = "61px";

const NavBarMobile: React.SFC<NavBarMobileProps> = ({
  children,
  leftItems,
  onPusherClick,
  onToggle,
  rightItems,
  visible,
  changeNavBarCard,
  height
}) => (
  <Sidebar.Pushable>
    <Sidebar as={Menu} animation="overlay" icon="labeled" inverted vertical direction="top" visible={visible}>
      {leftItems.map((item: NavBarItem) => (
        <Menu.Item {...item} onClick={() => changeNavBarCard(item.url)} />
      ))}
      {rightItems.map((item: NavBarItem) => (
        <Menu.Item {...item} onClick={() => changeNavBarCard(item.url)} />
      ))}
      <Menu.Item onClick={onToggle}>
        <Icon name="sidebar" />
      </Menu.Item>
    </Sidebar>
    <Sidebar.Pusher dimmed={visible} onClick={onPusherClick} style={{ minHeight: "100vh" }}>
      {console.log(typeof height)}
      <Menu fixed="top" inverted style={{ height: height ? height : defaultHeight }}>
        <Menu.Item>
          <Image size="mini" src="https://react.semantic-ui.com/logo.png" />
        </Menu.Item>
        <Menu.Item onClick={onToggle}>
          <Icon name="sidebar" />
        </Menu.Item>
        <Menu.Menu position="right">
          {rightItems.map((item: NavBarItem) => (
            <Menu.Item onClick={() => changeNavBarCard(item.url)} {...item} />
          ))}
        </Menu.Menu>
      </Menu>
      <div style={{ height: height ? height : defaultHeight }}></div>
      {children}
    </Sidebar.Pusher>
  </Sidebar.Pushable>
);

const NavBarDesktop: React.SFC<NavBarDesktopProps> = ({ leftItems, rightItems, changeNavBarCard, height, children }) => (
  <>
    <Menu fixed="top" inverted style={{ height: height ? height : defaultHeight }}>
      <Menu.Item>
        <Image size="mini" src="https://react.semantic-ui.com/logo.png" />
      </Menu.Item>
      {leftItems.map((item: NavBarItem) => (
        <Menu.Item onClick={e => changeNavBarCard(item.url)} {...item} />
      ))}
      <Menu.Menu position="right">
        {rightItems.map((item: NavBarItem) => (
          <Menu.Item onClick={e => changeNavBarCard(item.url)} {...item} />
        ))}
      </Menu.Menu>
    </Menu>
    <div style={{ height: height ? height : defaultHeight }} />
    {children}
  </>
);

class NavBar extends Component<RoutedNavBarProps> {
  state = {
    visible: false
  };

  handlePusher = () => {
    const { visible } = this.state;

    if (visible) this.setState({ visible: false });
  };

  changeNavBarCard = (url: string) => {
    this.props.history.push(url);
  };

  handleToggle = () => this.setState({ visible: !this.state.visible });

  render() {
    const { children, leftItems, rightItems, height } = this.props;

    return (
      <>
        <Responsive {...Responsive.onlyMobile}>
          <NavBarMobile
            leftItems={leftItems}
            onPusherClick={this.handlePusher}
            onToggle={this.handleToggle}
            rightItems={rightItems}
            visible={this.state.visible}
            changeNavBarCard={this.changeNavBarCard}
            height={height}
          >
            {children}
          </NavBarMobile>
        </Responsive>
        <Responsive minWidth={Responsive.onlyTablet.minWidth}>
          <NavBarDesktop leftItems={leftItems} rightItems={rightItems} height={height} changeNavBarCard={this.changeNavBarCard}>
            {children}
          </NavBarDesktop>
        </Responsive>
      </>
    );
  }
}

export default withRouter(NavBar);
