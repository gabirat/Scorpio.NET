import React from "react";

interface IGamepadStickProps {
  posX: number;
  posY: number;
}

export default function GamepadStick(props: IGamepadStickProps) {
  return (
    <svg viewBox="-2.2 -2.2 4.4 4.4" width="100" height="100">
      <circle cx="0" cy="0" r="2" fill="none" stroke="#888" strokeWidth="0.04" />
      <path d="M0,-2L0,2M-2,0L2,0" stroke="#888" strokeWidth="0.04" />
      <circle cx={props.posX * 2} cy={props.posY * 2} r="0.25" fill="red" className="axis" />
      <text textAnchor="middle" x="0" y="1.8">
        {props.posX.toFixed(2)} {props.posY.toFixed(2)}
      </text>
    </svg>
  );
}
