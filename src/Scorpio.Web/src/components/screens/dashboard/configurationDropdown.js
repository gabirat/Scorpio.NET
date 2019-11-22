import React from "react";
import { useSelector } from "react-redux";
import { Dropdown } from "semantic-ui-react";

const ConfigurationDropdown = ({ onSelect, onAddNew }) => {
  const configs = useSelector(x => x.configs);
  let pageConfigs = [];
  if (configs && Array.isArray(configs) && configs.length > 0) {
    pageConfigs = configs.filter(x => x.type === "page");
  }

  const onClick = (ev, config) => {
    ev.preventDefault();
    onSelect(config);
  };

  const onAddClick = ev => {
    ev.preventDefault();
    onAddNew();
  };

  return (
    <Dropdown text="Configuration" pointing="top" lazyLoad>
      <Dropdown.Menu>
        {pageConfigs.map(config => {
          return <Dropdown.Item label={config.name} key={`${config.id}${config.name}`} as="a" onClick={ev => onClick(ev, config)} />;
        })}
        <Dropdown.Item icon="add" label="New configuration" as="a" onClick={ev => onAddClick(ev)} />
      </Dropdown.Menu>
    </Dropdown>
  );
};

export default ConfigurationDropdown;
