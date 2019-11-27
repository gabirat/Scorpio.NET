import React from "react";
import PropTypes from "prop-types";
import { Pagination, Dropdown, Label } from "semantic-ui-react";

const Pager = ({ currentPage, itemsPerPage, onPageChange, onItemsPerPageChanged }) => {
  const handlePageChange = page => {
    if (typeof onPageChange === "function") {
      onPageChange(page);
    }
  };

  const handleItemsPerPageChanged = items => {
    if (typeof onItemsPerPageChanged === "function") {
      onItemsPerPageChanged(items);
    }
  };

  return (
    <div style={{ width: "100%", display: "flex", justifyContent: "center", alignItems: "center" }}>
      <Pagination
        style={{ marginLeft: "auto" }}
        onPageChange={(_, d) => handlePageChange(d.activePage)}
        boundaryRange={0}
        defaultActivePage={currentPage || 1}
        siblingRange={2}
        totalPages={20}
      />
      <div style={{ marginLeft: "auto" }}>
        <Label basic pointing="right">
          Per page:
        </Label>
        <Dropdown
          onChange={(_, d) => handleItemsPerPageChanged(d.value)}
          additionLabel="add"
          selection
          compact
          defaultValue={itemsPerPage || 20}
          options={[
            { key: 10, value: 10, text: 10 },
            { key: 25, value: 25, text: 25 },
            { key: 50, value: 50, text: 50 },
            { key: 75, value: 75, text: 75 },
            { key: 100, value: 100, text: 100 }
          ]}
        />
      </div>
    </div>
  );
};

Pager.propTypes = {
  currentPage: PropTypes.number.isRequired,
  itemsPerPage: PropTypes.number.isRequired,
  onPageChange: PropTypes.func.isRequired,
  onItemsPerPageChanged: PropTypes.func.isRequired
};

export default Pager;
