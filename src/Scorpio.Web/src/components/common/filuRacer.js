import React, { useState, useEffect } from "react";
import { Segment, Input, Button, Icon, Header } from "semantic-ui-react";

const FiluRacer = () => {
  const [player, setPlayer] = useState("");
  const [winner, setWinner] = useState("");

  const won = player => {
    setWinner(player);
  };

  return (
    <>
      {winner ? (
        <Award player={winner} />
      ) : (
        <>
          {player ? (
            <>
              <Runner player={"Filu"} won={won} />
              <Runner player={player} won={won} />
            </>
          ) : (
            <GetPlayer onStart={player => setPlayer(player)} />
          )}
        </>
      )}
    </>
  );
};

const Award = ({ player }) => {
  return (
    <Header textAlign="center" as="h1">
      Player {player} won!
    </Header>
  );
};

const Runner = ({ player, won }) => {
  const width = 800;
  const [pos, setPos] = useState(0);

  useEffect(() => {
    const interval = setInterval(() => {
      setPos(pos => {
        if (pos < width) {
          return pos + Math.floor(Math.random() * 5);
        } else {
          clearInterval(interval);
          won(player);
        }
      });
    }, 20);

    return () => {
      clearInterval(interval);
    };
  }, []);

  return (
    <div style={{ width: `${width}px`, height: "120px", margin: "auto", border: "2px solid gray", display: "flex", alignItems: "center" }}>
      <div style={{ position: "relative", left: pos }}>
        <Icon name="wheelchair" size="huge">
          <Header as="h3">{player}</Header>
        </Icon>
      </div>
    </div>
  );
};

const GetPlayer = ({ onStart }) => {
  const [player, setPlayer] = useState("");
  return (
    <Segment textAlign="center">
      <Input
        label="Enter your nickname"
        placeholder="Enter your nickname..."
        type="text"
        onChange={(ev, d) => setPlayer(d.value)}
        value={player}
      />
      <Button style={{ margin: "1rem" }} primary onClick={_ => onStart(player)}>
        Start
      </Button>
    </Segment>
  );
};

export default FiluRacer;
