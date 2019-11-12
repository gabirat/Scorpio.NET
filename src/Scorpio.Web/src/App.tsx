import React, { useState } from "react";
import "./App.css";
import { Range } from "react-range";
import "semantic-ui-css/semantic.min.css";
import MessagingService from "./services/messagingService";
import GamepadService from "./services/gamepad/gamepadService";

const App: React.FC = () => {
  const gamepad = new GamepadService();
  gamepad.init();

  const [values, setValues] = useState([0]);
  let messingService: MessagingService = MessagingService.getInstance();

  const handleClick = async (ev: React.MouseEvent): Promise<void> => {
    ev.preventDefault();
    messingService = MessagingService.getInstance();
    await messingService.connect();
    messingService.send("Data", [1]);
  };

  const handleSliderChange = async (values: number[]): Promise<void> => {
    setValues(values);

    messingService = MessagingService.getInstance();
    await messingService.connect();
    console.log(messingService, values);
    messingService.send("Data", values);
  };

  if (messingService !== undefined) console.warn(messingService!.message);

  return (
    <div className="App">
      <Range
        step={0.1}
        min={0}
        max={100}
        values={values}
        onChange={(v: number[]) => {
          handleSliderChange(v);
        }}
        renderTrack={({ props, children }: any) => (
          <div
            {...props}
            style={{
              ...props.style,
              height: "6px",
              width: "100%",
              backgroundColor: "#ccc"
            }}
          >
            {children}
          </div>
        )}
        renderThumb={({ props }: any) => (
          <div
            {...props}
            style={{
              ...props.style,
              height: "42px",
              width: "42px",
              backgroundColor: "#999"
            }}
          />
        )}
      />
      <div style={{ marginTop: "200px" }}>
        <Range
          step={0.1}
          min={0}
          max={100}
          values={messingService! ? [messingService!.message.PosX] : []}
          onChange={(v: number[]) => v.toString()}
          renderTrack={({ props, children }: any) => (
            <div
              {...props}
              style={{
                ...props.style,
                height: "6px",
                width: "100%",
                backgroundColor: "#bbb"
              }}
            >
              {children}
            </div>
          )}
          renderThumb={({ props }: any) => (
            <div
              {...props}
              style={{
                ...props.style,
                height: "42px",
                width: "42px",
                backgroundColor: "#999"
              }}
            />
          )}
        />
      </div>
      <button
        onClick={ev => {
          handleClick(ev);
        }}
      >
        ASDasdasd
      </button>
    </div>
  );
};

export default App;
