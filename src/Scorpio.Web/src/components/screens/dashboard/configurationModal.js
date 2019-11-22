import React, { useState } from "react";
import { Button, Header, Modal, Input, Form, Container } from "semantic-ui-react";

const ConfigurationModal = ({ onClose, onAdd }) => {
  const [text, setText] = useState("");
  const [loading, setLoading] = useState(false);

  const handleSubmit = ev => {
    ev.preventDefault();
    setLoading(true);
    if (onAdd && typeof onAdd === "function") onAdd(text);
  };

  return (
    <Modal size="tiny" dimmer={"inverted"} open={true} onClose={onClose}>
      <Modal.Content>
        <Modal.Description>
          <Header>Add new dashboard page</Header>
          <Container>
            <Form loading={loading}>
              <Form.Field>
                <Input
                  type="text"
                  name="pageName"
                  label="Page name"
                  placeholder="Enter new page name..."
                  value={text}
                  onChange={(ev, d) => setText(d.value)}
                />
              </Form.Field>
              <Button onClick={handleSubmit}>Create</Button>
            </Form>
          </Container>
        </Modal.Description>
      </Modal.Content>
    </Modal>
  );
};

export default ConfigurationModal;
