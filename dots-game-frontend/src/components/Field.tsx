import React from "react";
import { Player } from "../types/types";
import "./Field.css";

type FieldProps = {
  player: Player;
  enclosedBy: Player;
};

const Field: React.FC<FieldProps> = ({ player, enclosedBy }) => {
  const getClassName = () => {
    if (enclosedBy == Player.None) {
      if (player == Player.None) return "field free";
      return player === Player.Human ? "field human" : "field ai";
    }
    else{
        if(player == Player.None) return "field empty-enclosed"
        return enclosedBy === Player.Human ? "field ai-enclosed-by-human" : "field human-enclosed-by-ai";
    }
  };

  return <div className={getClassName()} />;
};

export default Field;
