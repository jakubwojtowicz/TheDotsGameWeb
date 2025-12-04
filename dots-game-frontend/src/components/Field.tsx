import React from "react";
import { Player } from "../types/dotsApiTypes";
import "./Field.css";

type FieldProps = {
  player: Player;
  enclosedBy: Player;
  row: number;
  col: number;
  onClick?: (row: number, col: number) => void;
  isLastAIMove?: boolean; // new prop
};

const Field: React.FC<FieldProps> = ({
  player,
  enclosedBy,
  row,
  col,
  onClick,
  isLastAIMove,
}) => {
  const getClassName = () => {
    let baseClass = "";

    if (enclosedBy === Player.None) {
      if (player === Player.None) baseClass = "field free";
      else baseClass = player === Player.Human ? "field human" : "field ai";
    } else {
      if (player === Player.None) baseClass = "field empty-enclosed";
      else
        baseClass =
          enclosedBy === Player.Human
            ? "field ai-enclosed-by-human"
            : "field human-enclosed-by-ai";
    }

    if (isLastAIMove) baseClass += " ai-last-move";

    return baseClass;
  };

  return <div className={getClassName()} onClick={() => onClick?.(row, col)} />;
};

export default Field;
