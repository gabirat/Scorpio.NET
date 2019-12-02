import React from "react";

const ListRenderer = ({ list, idKey, textKey }) => {
  if (!list && list.length === 0) return null;
  return (
    <ul style={{ padding: 0, listStyle: "none" }}>
      {list.map(item => {
        return <li key={item[idKey]}>{item[textKey]}</li>;
      })}
    </ul>
  );
};

export default ListRenderer;
