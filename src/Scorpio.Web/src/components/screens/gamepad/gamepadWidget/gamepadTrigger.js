import React from "react";

export default function GamepadTrigger(props) {
  return (
    <span className="gamepad_svg">
      <svg c viewBox="-1.2 -2.2 1.4 4.4" width="50" height="100">
        <rect x="-0.5" y="-2.0" width="1" height="4" fill="red" stroke="#888" strokeWidth="0.06" />
        <rect x="-0.5" y="-2.0" width="1" height={4 - props.value * 4} fill="white" stroke="#red" />
        <text textAnchor="middle" x="0" y="1.8">
          {props.value.toFixed(2)}
        </text>
      </svg>
    </span>
  );
}
